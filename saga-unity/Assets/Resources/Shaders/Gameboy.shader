Shader "SaGa/Gameboy"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _Black("Black in", Color) = (0,0,0,1)
        _DGray("Dark Gray in", Color) = (.226,.226,.226,1)
        _LGray("Light Gray in", Color) = (.668,.668,.668,1)
        _White("White in", Color) = (1,1,1,1)
        _BlackOut("Black out", Color) = (0,0,0,1)
        _DGrayOut("Dark Gray out", Color) = (.226,.226,.226,1)
        _LGrayOut("Light Gray out", Color) = (.668,.668,.668,1)
        _WhiteOut("White out", Color) = (1,1,1,1)
	}
	SubShader
	{
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
    
		Cull Off 
        ZTest Always
        ZWrite Off
        Blend One OneMinusSrcAlpha

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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            fixed4 _Black;
            fixed4 _DGray;
            fixed4 _LGray;
            fixed4 _White;
            fixed4 _BlackOut;
            fixed4 _DGrayOut;
            fixed4 _LGrayOut;
            fixed4 _WhiteOut;

            float color_d(fixed4 c1, fixed4 c2) {
                return abs(c1[0] - c2[0]) + abs(c1[1] - c2[1]) + abs(c1[2] - c2[2]);
            }

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 current = tex2D(_MainTex, i.uv);
                current.rgb *= current.a;

                float dBlack = color_d(current, _Black);
                float dDgray = color_d(current, _DGray);
                float dLgray = color_d(current, _LGray);
                float dWhite = color_d(current, _White);
                
                if (current.a > 0.0) {
                    if (dBlack <= dDgray && dBlack <= dLgray && dBlack <= dWhite) {
                        return _BlackOut;
                    }
                    if (dDgray <= dBlack && dDgray <= dLgray && dDgray <= dWhite) {
                        return _DGrayOut;
                    }
                    if (dLgray <= dBlack && dLgray <= dDgray && dLgray <= dWhite) {
                        return _LGrayOut;
                    }
                    if (dWhite <= dBlack && dWhite <= dDgray && dWhite <= dLgray) {
                        return _WhiteOut;
                    }
                }
				return current;
			}
			ENDCG
		}
	}
}
