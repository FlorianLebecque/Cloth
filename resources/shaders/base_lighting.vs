#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec4 vertexColor;
// added
in vec4 vertexTangent;

// Input uniform values
uniform mat4 mvp;

// Output vertex attributes (to fragment shader)
out vec2 fragTexCoord;
out vec4 fragColor;
// added
out vec3 fragPosition;
out vec3 fragNormal;


// NOTE: Add here your custom variables

void main()
{
    // Send vertex attributes to fragment shader
    fragTexCoord = vertexTexCoord;
    fragColor = vertexColor;
    //added
    fragNormal = vertexNormal;
    fragPosition = vertexPosition;


    // Calculate final vertex position
    gl_Position = mvp*vec4(vertexPosition, 1.0);
}