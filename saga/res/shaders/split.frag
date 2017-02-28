#ifdef GL_ES
precision mediump float;
#endif

varying vec4 v_color; 
varying vec2 v_texCoords; 
uniform sampler2D u_texture;
uniform float u_elapsed;

float rand(vec2 co){
	return fract(sin(dot(co.xy ,vec2(12.9898, 78.233))) * 43758.5453);
}

void main() {
    float height = 0.6;
    
    float adjElapsed = (u_elapsed * u_elapsed * u_elapsed) * 2.0;
	float offX = 0;
    float y = 1.0 - (v_texCoords[1] / height - .5);
    float x = v_texCoords[0];
    if (adjElapsed > (y - 0.2)) {
        float term = adjElapsed - (y - 0.2);
        offX = (term * term + term) * 3.0 / 4.0;
    }
   
    if (v_texCoords[0] > .5) {
        offX = offX * -1.0;
    }
    if (mod(gl_FragCoord[1], 4.0) >= 2.0) {
        offX = offX * 5.0 / 3.0;
    }
    
	gl_FragColor = v_color * texture2D(u_texture, v_texCoords + vec2(offX, 0));
}