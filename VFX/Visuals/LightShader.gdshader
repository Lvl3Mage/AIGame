shader_type canvas_item;

varying vec4 original_color;
varying vec2 worldUV;
uniform vec4 shaded_color : source_color;
uniform float featherWallsRadius = 16.0;
uniform float light_influence = 1.0;
uniform sampler2D light_noise1;
uniform sampler2D light_noise2;
void vertex() {
	worldUV = (MODEL_MATRIX*vec4(VERTEX, 0.0, 1.0)).xy;
	// Called for every vertex the material is visible on.
}

vec2 wrapUV(vec2 uv) {
	return fract(uv);
}
vec3 sampleNoise(sampler2D tex,vec2 coords, float scale) {
	return texture(tex, wrapUV(coords * scale*0.01)).rgb;
}

vec3 compute(vec2 coords){

	vec3 noise1 = sampleNoise(light_noise1, coords + vec2(0.6,0.4)*TIME*3.0, 1.0);
	vec3 noise2 = sampleNoise(light_noise2, coords + vec2(-1.0,0.3)*TIME*2.0, 1.0);
	return noise1*noise2;
}
void fragment() {
	original_color = COLOR;
	COLOR = mix(COLOR,vec4(shaded_color.rgb, COLOR.a),light_influence);
	// Called for every pixel the material is visible on.
}
float feather_decay(float distance, float radius) {
	return 1.0 - exp(-distance * radius);
}

float quantize(float v, float s) {
	return round(v / s) * s;
}

void light() {
	vec2 coords = worldUV.xy*0.05;
	vec3 modulation = compute(coords);
	float noise_modulation = (modulation.r -0.5)*2.0;

	vec4 light = LIGHT_COLOR;
	float light_average = (light.r + light.g + light.b) / 3.0;
	float light_mask = step(0.1, light_average + noise_modulation*0.1);
	LIGHT = original_color*light * light_mask * LIGHT_ENERGY;
	LIGHT = min(LIGHT, vec4(1.0, 1.0, 1.0, 1.0));



	vec2 sdf_uv = screen_uv_to_sdf(SCREEN_UV);
	float sdf_value = texture_sdf(sdf_uv);

	float sdf_attenuation = feather_decay(sdf_value,featherWallsRadius);//smoothstep(0.0, 1, sdf_value / featherWallsRadius);
	sdf_attenuation = clamp(sdf_attenuation, 0.0, 1.0);
	LIGHT.rgb *= vec3(sdf_attenuation);
	//SHADOW_MODULATE = shaded_color;
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
}
