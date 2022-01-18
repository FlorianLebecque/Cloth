#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform vec2 resolution;

// Output fragment color
out vec4 finalColor;

// NOTE: Add here your custom variables


const float Distance = 10.0;          // Pixels per axis; higher = bigger glow, worse performance
const float quality = 10;            // Defines size factor: Lower = smaller glow, better quality

const float TWO_PI = 6.28318531;
const float ANGLE_INC = TWO_PI/20;
const float DIST_INC = 1;

void main()
{
    vec4 sum = vec4(0);
    vec2 sizeFactor = vec2(quality)/(resolution);

    // Texel color fetching from texture sampler
    vec4 source = texture(texture0, fragTexCoord);

    for(float a = 0; a < TWO_PI ; a+= ANGLE_INC){
        for(float d = 0; d < Distance; d += DIST_INC){

            int x = int( d * cos(a) );
            int y = int(-d * sin(a) );

            sum += texture(texture0, fragTexCoord + vec2(x, y)*sizeFactor);

        }
    }

    float nbr = (TWO_PI/ANGLE_INC) * (Distance/DIST_INC);

    // Calculate final fragment color
    finalColor = ((sum/(nbr)) + source)*colDiffuse;
}