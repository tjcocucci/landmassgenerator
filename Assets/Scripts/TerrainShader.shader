Shader "Custom/TerrainShader"
{
    Properties
    {}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int maxColorLevels = 8;
        int countColorLevels;
        float3 colors[maxColorLevels];
        float baseHeights[maxColorLevels];

        float minHeight;
        float maxHeight;

        struct Input
        {
            float3 worldPos;
        };

        float inverseLerp (float a, float b, float value) {
            return saturate((value - a) / (b - a));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float percent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
            for (int i = 0; i < countColorLevels; i++) {
                float drawStrength = saturate(sign(percent - baseHeights[i]));
                o.Albedo = o.Albedo * (1 - drawStrength) + colors[i] * drawStrength;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
