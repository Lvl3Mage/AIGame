shader_type canvas_item;

uniform vec4 color : source_color; // Tint color of the vignette
uniform float radius = 0.4;        // Radius of the vignette circle
uniform float softness = 0.2;      // How soft the edge is

void fragment() {
    float value = distance(UV, vec2(0.5));
    float alpha = smoothstep(radius, radius + softness, value);
    COLOR = vec4(color.rgb, alpha * color.a);
}
