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
    float offY = -1.0 * u_elapsed;
    float offX = rand(vec2(u_elapsed, 0.0)) - 0.5;
    offX *= 0.15;
    
	gl_FragColor = v_color * texture2D(u_texture, v_texCoords + vec2(offX, offY));
    
    float brightOff = -0.5 * u_elapsed;
    if (v_texCoords[1] > 0.9) {
        brightOff = (v_texCoords[1] - 0.9) * 10.0 * 1.5;
    }
    if (v_texCoords[1] + offY > 1.0 || v_texCoords[1] + offY < 0.0) {
        brightOff = 2.0;
    }
    if (v_texCoords[0] + offX > 1.0 || v_texCoords[0] + offX < 0.0) {
        brightOff = 2.0;
    }
    gl_FragColor[0] += brightOff;
    gl_FragColor[1] += brightOff;
    gl_FragColor[2] += brightOff;
}