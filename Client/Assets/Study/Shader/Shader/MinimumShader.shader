Shader "Custom/MinimumShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = fixed4(1.0f, 1.0f, 1.0f, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
