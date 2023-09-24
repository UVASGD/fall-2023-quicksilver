void CelShading_float(in float3 Normal, in float CelRampSmoothness, in float3 ClipSpacePos, in float3 WorldPos, in float4 CelRampTinting,
in float CelRampOffset, out float3 CelRampOutput, out float3 Direction)
{
    #ifdef SHADERGRAPH_PREVIEW
        CelRampOutput = float3(0.5,0.5,0);
        Direction = float3(0.5,0.5,0);
    #else
        #if SHADOWS_SCREEN
            half4 shadowCoord = ComputerScreenPos(ClipSpacePos);
        #else
            half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif

        #if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
            Light light = GetMainLight(shadowCoord);
        #else
            Light light = GetMainLight();
        #endif

        half d = dot(Normal, light.direction) * 0.5 + 0.5;

        half celRamp = smoothstep(CelRampOffset, CelRampOffset + CelRampSmoothness, d);

        celRamp *= light.shadowAttenuation;

        CelRampOutput = light.color * (celRamp + CelRampTinting);

        Direction = light.direction;
    #endif
}