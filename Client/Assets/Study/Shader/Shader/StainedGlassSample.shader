Shader "Custom/StainedGlassSample"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex,IN.uv_MainTex);
            o.Albedo = tex.rgb;

            // グレースケール
            // http://www.dinop.com/vc/image_gray_scale.html
            o.Alpha = (tex.r*0.3 + tex.g*0.6 + tex.b*0.1 < 0.2) ? 1 : 0.7;
            //o.Alpha = (tex.r * 77 + tex.g * 150 + tex.b * 29 < 0.2)>>8;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
