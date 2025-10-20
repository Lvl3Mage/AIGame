shader_type canvas_item;

uniform float outerRadius : hint_range(0.0, 5.0) = 1.0;
uniform float MainAlpha : hint_range(0.0, 1.0) = 1.0;
uniform float Posterization= 10.0;
uniform vec3 color1: source_color = vec3(1,1,1);
uniform vec3 color2: source_color = vec3(1,1,1);
uniform float PixelationScale = 300.0;
uniform float HighlightNoiseMin = 1.0;
uniform float HighlightNoiseMax = 1.05;

uniform float EdgeSpread: hint_range(0, 1) = 0.6;
#include "res://VFX/Shaders/voronoi.gdshaderinc"
#include "res://addons/shaderV/rgba/posterize.gdshaderinc"
float uniformFloatFromVec2(vec2 p) {
    uvec2 u = floatBitsToUint(p);
    uint h = u.x * 374761393u + u.y * 668265263u;
    h = (h ^ (h >> 13u)) * 1274126177u;
    return float(h & 0x00FFFFFFu) / float(0x01000000u);
}

void fragment() {
	vec2 screen = UV;
	screen = round(screen*PixelationScale)/PixelationScale;
	screen.y *= SCREEN_PIXEL_SIZE.x/ SCREEN_PIXEL_SIZE.y;

	VoronoiData cellNoise = AdvancedVoronoi(screen, TIME,100);
	VoronoiData warpNoise = AdvancedVoronoi(screen, TIME,10);
	vec2 cellUV = cellNoise.closestCellPos;
	cellUV.y *= SCREEN_PIXEL_SIZE.y/ SCREEN_PIXEL_SIZE.x;
	float x = abs(cellUV.x-.5)*2.0 + warpNoise.centerDistance*0.1;
	float y = abs(cellUV.y-.5)*2.0 + warpNoise.centerDistance*0.1;
	float hash = uniformFloatFromVec2(cellNoise.closestCellPos);

	float v = (sqrt((x*x)+(y*y))/outerRadius);
	v = smoothstep(EdgeSpread,1.0,v);

	vec3 s = _posterizeFunc(vec3(v,v,v),Posterization);
	float alfa = s.r*MainAlpha;


	float screenNoise = uniformFloatFromVec2(screen);
	screenNoise = mix(HighlightNoiseMin,HighlightNoiseMax,screenNoise);
	vec3 clr = mix(color1,color2,hash)*screenNoise ;
	COLOR = vec4(clr,s.r*MainAlpha);
}
