Shader "Custom/RimLightSample"
{
    Properties{
        _baseColor("Color",Color)=(1,1,1,1)
        _rimColor("Rim Color",Color)=(0.5, 0.7, 0.5, 1)
        _rimPow("Rim Pow",float)=3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
        };

        fixed4 _baseColor;
        fixed4 _rimColor;
        float _rimPow;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _baseColor;
            float rim = 1 - saturate(dot(IN.viewDir, o.Normal));
            //o.Emission = rimCr * pow(rim, 2.5);
            //o.Emission = rimCr * rim;
            o.Emission = _rimColor * pow(rim, _rimPow);
        }
        ENDCG
    }

    FallBack "Diffuse"
}
