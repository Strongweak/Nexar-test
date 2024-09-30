Shader "Unlit/TestShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _A ("A", Range(0.0, 1.0)) = 0.6
        _B ("B", Range(0.0, 1.0)) = 0.8
        _Smoothing ("Smoothing",Range(0.0, 0.2)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        //no need LOD
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color: COLOR;
            };

            sampler2D _MainTex;
            float _A;
            float _B;
            float _Smoothing;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float x = i.uv.x;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //reverted color
                fixed4 revertCol = fixed4(col.g, col.b, col.r, col.a);
                //color * (0.299,0.587,0.114): determine brightness of rgb color 
                float grayScale = dot(col.rgb, float3(0.299, 0.587, 0.114));
                //combine color
                float4 blend = revertCol * grayScale;
                
                float aSmear = smoothstep(_A - _Smoothing, _A + _Smoothing, x);
                float bSmear = smoothstep(_B - _Smoothing, _B + _Smoothing, x);

                fixed4 finalCol = lerp(grayScale, revertCol, aSmear);
                finalCol = lerp(finalCol, blend, bSmear);
                
                return finalCol;
            }
            ENDCG
        }
    }
}