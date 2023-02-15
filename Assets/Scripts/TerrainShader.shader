Shader "Custom/TerrainShader"
{
    Properties
    { }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float textureStrength;
        const static int maxColorLevels = 8;
        int countColorLevels;
        float3 colors[maxColorLevels];
        float baseHeights[maxColorLevels];
        float blends[maxColorLevels];

        UNITY_DECLARE_TEX2DARRAY(textures);
        float textureScales[maxColorLevels];

        float minHeight;
        float maxHeight;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverseLerp (float a, float b, float value) {
            return saturate((value - a) / (b - a));
        }

        float3 triplanar(float3 worldPos, float texScale, float3 normal, int levelIndex) {
            float3 scaledWorldPos = worldPos / texScale;
            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(textures, float3(scaledWorldPos.y, scaledWorldPos.z, levelIndex)) * normal.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(textures, float3(scaledWorldPos.x, scaledWorldPos.z, levelIndex)) * normal.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(textures, float3(scaledWorldPos.x, scaledWorldPos.y, levelIndex)) * normal.z;
            return xProjection + yProjection + zProjection;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float percent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
            float3 normal = abs(IN.worldNormal);
            normal /= normal.x + normal.y + normal.z;
            float3 colorValue = float3(0, 0, 0);
            float3 texValue = float3(0, 0, 0);
            for (int i = 0; i < countColorLevels; i++) {
                float drawStrength = inverseLerp(-blends[i]/2 - 1E-4, blends[i]/2, percent - baseHeights[i]);
                colorValue = colorValue * (1 - drawStrength) + colors[i] * drawStrength;
                texValue = texValue * (1 - drawStrength) + triplanar(IN.worldPos, textureScales[i], normal, i) * drawStrength;
                // float3 texValue = texValue + triplanar(IN.worldPos, textureScales[i], normal, i);
                // solo una textura por iteracion
                o.Albedo = colorValue * (1 - textureStrength) + texValue * textureStrength; 
            }

        }
        ENDCG
    }
    FallBack "Diffuse"
}
