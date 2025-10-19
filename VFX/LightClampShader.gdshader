shader_type canvas_item;

varying vec4 original_color;
uniform vec4 new_color : source_color;
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	original_color = COLOR;
	COLOR *= vec4(new_color.rgb, COLOR.a);
// 	COLOR *= 0.5;Z
	// Called for every pixel the material is visible on.
}

void light() {
	LIGHT = original_color*LIGHT_COLOR * LIGHT_ENERGY;

	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
}
