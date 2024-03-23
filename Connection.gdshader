shader_type canvas_item;

uniform vec2 label_startpos;
uniform vec2 label_size;
uniform vec2 texture_size;

void fragment() {
	
	if (UV.x * texture_size.x >= label_startpos.x && UV.y * texture_size.y >= label_startpos.y && UV.x * texture_size.x <= (label_startpos.x + label_size.x) && UV.y * texture_size.y <= (label_startpos.y + label_size.y))
	{ COLOR.a = 0.0; }
	else { COLOR.a = 1.0; }
}