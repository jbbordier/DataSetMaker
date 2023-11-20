Shader "Custom/HaloShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (.002,.03)) = .005
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _OutlineColor;
        float _Outline;

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            // Add outline
            float4 outline = tex2D(_MainTex, IN.uv_MainTex);
            clip(outline.a - 0.5);

            // Add red or blue outline based on color
            fixed4 finalOutlineColor = _OutlineColor;
            if (c.r > 0.5) // Check if the pixel color is closer to red
            {
                finalOutlineColor = float4(1, 0, 0, 1); // Red
            }
            else
            {
                finalOutlineColor = float4(0, 0, 1, 1); // Blue
            }

            o.Outline = finalOutlineColor * _Outline;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
