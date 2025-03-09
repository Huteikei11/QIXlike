Shader "Custom/MaskShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _MinBounds ("Min Bound", Vector) = (0,0,0,0)
        _MaxBounds ("Max Bound", Vector) = (1,1,1,1)
        _MaskColor ("Mask Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // シェーダーで使用する変数
            float4 _MinBounds;
            float4 _MaxBounds;
            float4 _MaskColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // 頂点シェーダー
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // フラグメントシェーダー
            half4 frag(v2f i) : SV_Target
            {
                // マスク領域内かどうかを判定
                if (i.uv.x >= _MinBounds.x && i.uv.x <= _MaxBounds.x &&
                    i.uv.y >= _MinBounds.y && i.uv.y <= _MaxBounds.y)
                {
                    // マスク範囲内ならオリジナルのテクスチャを表示
                    return tex2D(_MainTex, i.uv);
                }
                else
                {
                    // マスク範囲外なら透明
                    return half4(0, 0, 0, 0);
                }
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
