Shader "Custom/SampleIceShader"
{
    Properties
    {
        _alpha("Alpha", Range(0,5)) = 1.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float3 worldNormal;
            float3 viewDir;
        };
        float _alpha;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = fixed4(1,1,1,1);
            float alpha = 1 - (abs(dot(IN.viewDir,IN.worldNormal)));
            o.Alpha = alpha*_alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
