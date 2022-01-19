#version 330

// Input vertex attributes (from vertex shader)
in vec3 fragPosition;
in vec2 fragTexCoord;
in vec4 fragColor;
in vec3 fragNormal;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform mat4 matModel;

// Output fragment color
out vec4 finalColor;

// NOTE: Add here your custom variables

#define     MAX_LIGHTS              4
#define     LIGHT_DIRECTIONAL       0
#define     LIGHT_POINT             1

struct MaterialProperty {
    vec3 color;
    int useSampler;
    sampler2D sampler;
};

struct Light {
    int enabled;
    int type;
    vec3 position;
    vec3 target;
    vec4 color;
};

// Input lighting values
uniform Light lights[MAX_LIGHTS];
uniform vec4 ambient;
uniform vec3 viewPos;


vec3 unHomogenous(vec4 v)
{
    // replaces vec3(matModel * vec4(fragPosition, 1.0)));
    return v.xyz/v.w;
}

void main()
{

    // Texel color fetching from texture sampler
    vec4 texelColor = texture(texture0, fragTexCoord);
    vec3 lightDot = vec3(0.0);

    mat3 matNormal =  transpose(inverse(mat3(matModel)));
    vec3 worldNormal  = normalize( matNormal * fragNormal);//normalize(fragNormal);

    vec3 worldPosition = unHomogenous(matModel * vec4(fragPosition,1));

    vec3 viewD = normalize(viewPos - worldPosition);
    vec3 specular = vec3(0.0);


    // NOTE: Implement here your fragment shader code

    for (int i = 0; i < MAX_LIGHTS; i++) {
        if (lights[i].enabled == 1) {

            vec3 lightDir  = normalize(lights[i].position - worldPosition);
            
            float dist = distance(worldPosition,lights[i].position);
            if(dist >= 1000){
                dist = 0;
            }else{
                dist = 1- ((dist)/1000);
            }

            lightDot += lights[i].color.rgb * clamp(dot(worldNormal, lightDir), 0.0, 0.5) * dist;

            vec3 reflectDir = reflect(-lightDir, worldNormal);  
            vec3 viewDir = normalize(viewPos - worldPosition);

            
            specular += pow(clamp(dot(viewDir, reflectDir), 0.1, 1), 16) * dist;
        }
    }

    finalColor = (texelColor*colDiffuse) * (vec4(lightDot,1.0) + vec4(specular,1.0));

    

    // Gamma correction
    finalColor = pow(finalColor, vec4(1.0/2.2));
}