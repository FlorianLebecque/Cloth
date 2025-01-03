#version 430

// Sphere mesh attributes
layout(location = 0) in vec3 aSphereVertex;    // Sphere template vertices 
layout(location = 1) in vec3 aInstancePos;     // Instance positions

uniform mat4 uProjection;
uniform mat4 uView;
uniform float uRadius = 0.5;

out vec3 vNormal;
out vec3 vPosition;

void main() {
    // Transform sphere template vertex by instance position
    vec3 worldPos = (aSphereVertex * uRadius) + aInstancePos;
    vPosition = worldPos;
    vNormal = normalize(aSphereVertex); // Unit sphere normals = position
    gl_Position = uProjection * uView * vec4(worldPos, 1.0);
}