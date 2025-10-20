// Helper: randomVector
// Note: Changed input from UV (float2) to cell_coord (ivec2) for clarity,
// as random values are typically based on integer cell coordinates.
vec2 randomVector (ivec2 cell_coord, float offset_param) { // Renamed 'offset' to 'offset_param' to avoid conflict if used as varying
    mat2 m = mat2(vec2(15.27, 99.41), vec2(47.63, 89.98)); // Column-major constructor
    vec2 uv_for_random = vec2(cell_coord); // Convert ivec2 to vec2 for calculations
    uv_for_random = fract(sin(uv_for_random * m) * 46839.32); // mul(UV,m) -> UV * m
    return vec2(sin(uv_for_random.y * offset_param) * 0.5 + 0.5, cos(uv_for_random.x * offset_param) * 0.5 + 0.5);
}

struct VoronoiData{
	float centerDistance;
	vec2 centerDerivative;
	float edgeDistance;
	vec2 edgeDerivative;
	vec2 closestCellPos;
};
// Main Voronoi function
// Changed 'out' parameters to 'inout'
VoronoiData AdvancedVoronoi(vec2 point, float p_AngleOffset, float p_CellDensity) {
    VoronoiData result;

    ivec2 cell = ivec2(floor(point * p_CellDensity));
    vec2 posInCell = fract(point * p_CellDensity);

    float primaryDistanceSqr = 8.0;
    vec2 primaryDelta = vec2(0.0);

    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
            ivec2 neighbor_cell_coord = cell + ivec2(i, j);
            vec2 delta = (vec2(float(i), float(j)) + randomVector(neighbor_cell_coord, p_AngleOffset)) - posInCell;
            float sqrDistance = dot(delta, delta);
            if (sqrDistance < primaryDistanceSqr) {
                primaryDelta = delta;
                primaryDistanceSqr = sqrDistance;
                result.closestCellPos = vec2(neighbor_cell_coord);
            }
        }
    }
	result.closestCellPos += vec2(0.5,0.5);
	result.closestCellPos /= p_CellDensity;

    float primaryDistance = sqrt(primaryDistanceSqr);
    result.centerDistance = primaryDistance;

    if (primaryDistance > 0.00001) {
        result.centerDerivative = -(primaryDelta / primaryDistance) * p_CellDensity;
    } else {
        result.centerDerivative = vec2(0.0);
    }

    float opposingDistanceSqr = 200.0;
    vec2 opposingDelta = vec2(0.0);

    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
            ivec2 neighbor_cell_coord = cell + ivec2(i, j);
            vec2 delta = (vec2(float(i), float(j)) + randomVector(neighbor_cell_coord, p_AngleOffset)) - posInCell;
            float sqrDistance = dot(delta, delta);
            bool is_not_primary = !(delta.x == primaryDelta.x && delta.y == primaryDelta.y);

            if (sqrDistance < opposingDistanceSqr && is_not_primary) {
                opposingDistanceSqr = sqrDistance;
                opposingDelta = delta;
            }
        }
    }

    vec2 pointDelta = primaryDelta - opposingDelta;
    vec2 midpoint = (primaryDelta + opposingDelta) * 0.5;

    result.edgeDistance = dot(-midpoint, pointDelta);

    float pointDeltaLength = length(pointDelta);
    if (pointDeltaLength > 0.00001) {
        result.edgeDerivative = normalize(pointDelta) * p_CellDensity;
    } else {
        result.edgeDerivative = vec2(0.0);
    }

    return result;
}
