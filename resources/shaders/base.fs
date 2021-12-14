#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;
in vec3 fragNormal;
in vec3 fragPosition;
in vec4 fragTangent;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform vec3 lightPos;
uniform mat4 matModel;


// Output fragment color
out vec4 finalColor;

// NOTE: Add here your custom variables

vec3 unHomogenous(vec4 v)
{
    return v.xyz/v.w;
}

void main()
{
    vec4 texelColor = texture(texture0, fragTexCoord*5);

    // Only keep the rotation (and scaling). It only works if scaling is uniform.
    // Otherwise matNormal is transpose(inverse(matModel)) which is more compute heavy
    mat3 matNormal = mat3(matModel);

    vec3 worldPosition = unHomogenous(matModel * vec4(fragPosition, 1.0));
    vec3 worldNormal = normalize(matNormal * fragNormal);

    vec3 lightDir = normalize(lightPos - worldPosition);
    
    float shading = clamp(dot(worldNormal, lightDir), 0.3, 1.0);
    
    finalColor = (texelColor*colDiffuse)*vec4(shading, shading, shading, 1.0);
}
