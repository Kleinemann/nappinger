shader_type canvas_item;

uniform vec2 shadow_offset = vec2(5.0, 5.0); // Shadow position offset
uniform float shadow_alpha = 0.5;            // Shadow transparency

void fragment() {
    // Sample the original texture
    vec4 tex_color = texture(TEXTURE, UV);
    
    // If transparent pixel, discard
    if (tex_color.a < 0.01) {
        discard;
    }

    // Draw shadow color (black with alpha)
    COLOR = vec4(0.0, 0.0, 0.0, shadow_alpha) * tex_color.a;
}
