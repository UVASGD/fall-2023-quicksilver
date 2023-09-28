#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// Sample offsets for each cell in matrix, relative to central pixel
static float2 sobelSamplePoints[9] = {
    float2(-1,1), float2(0,1), float2(1,1),
    float2(-1,0), float2(0,0), float2(1,1),
    float2(-1,-1), float2(0,-1), float2(1,-1),
};

//Weights for x Component
static float sobelXMatrix[9] = {
    2, 0, -2,
    4, 0, -4,
    2, 0, -2,
};

//Weights for y component
static float sobelYMatrix[9] = {
    2,  4,  2,
    0,  0,  0,
   -2, -4, -2,
};

void DepthSobel_float(float2 UV, float Thickness, out float Out) {
    float2 sobel = 0;

    [unroll] for (int i = 0; i < 9; i++) {
        float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
        sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
    }

    Out = length(sobel);
}

#endif