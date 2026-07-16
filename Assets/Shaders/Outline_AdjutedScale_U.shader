Shader "AkaneShader/Unlit/Outline_AdjutedScale"
{
    Properties{
         [MainTexture] _MainTex ("Texture", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 1
    }

    SubShader
    {
        Tags{ 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        //テクスチャの表示を行うシェーダーパス
        Pass
        {
            Name "SampleTexture"

            Stencil{
                Ref 1
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                //オブジェクトのスケールを取得
                float4x4 m = GetObjectToWorldMatrix();
                float scaleX = length(float3(m._11, m._21, m._31));
                float scaleY = length(float3(m._12, m._22, m._32));
                float scaleZ = length(float3(m._13, m._23, m._33));

                //絶対値を取ることで正負関係なく面を含める
                float isTop = abs(v.normal.y) > 0.9;
                float isXSide = abs(v.normal.x) > 0.9;

                //オブジェクトのスケールにuvを合わせる
                if(isTop){//y面
                    v.uv *= float2(scaleX, scaleZ);
                }
                else if(isXSide){//x面
                    v.uv *= float2(scaleZ, scaleY);
                }
                else{//z面
                    v.uv *= float2(scaleX, scaleY);
                }

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return col;
            }
            ENDHLSL
        }

        //アウトラインを表示するパス
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attribute
            {
                float4 vertex : POSITION;
                float3 normal : TEXCOORD3;
            };

            struct FragAttribute
            {
                float4 vertex : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            FragAttribute vert (Attribute v)
            {
                FragAttribute o;

                //頂点の法線をワールド空間->クリップ空間へと変換する
                float3 normalWS = normalize(TransformObjectToWorldDir(v.normal));
                float3 normalCS = TransformWorldToHClipDir(normalWS);
                
                //頂点を法線方向に押し出す
                float4 positionCS = TransformObjectToHClip(v.vertex.xyz);
                o.vertex = positionCS + float4(normalCS.xy * _OutlineWidth, 0, 0);

                return o;
            }

            half4 frag (FragAttribute i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
