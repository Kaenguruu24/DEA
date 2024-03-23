shader_type canvas_item;

uniform sampler2D texture;
uniform sampler2D alpha_texture;

void fragment() {
    vec4 color = texture(texture, UV);
    vec4 alpha_color = texture(alpha_texture, UV);
    color.a *= alpha_color.a;

    COLOR = color;
}