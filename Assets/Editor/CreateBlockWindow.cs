using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AkaneTools
{
    public class CreateBlockWindow : EditorWindow
    {
        private const float MINIMUM_SCALE_VALUE = 0.01f;
        private const float MAXIMUM_SCALE_VALUE = 90f;

        [SerializeField]
        private GameObject _baseObject = null;
        [SerializeField, Range(MINIMUM_SCALE_VALUE, MAXIMUM_SCALE_VALUE)]
        private Vector3 _scale = Vector3.one;
        [SerializeField]
        private Material _material = null;
        [SerializeField]
        private string _prefixName = string.Empty;
        [SerializeField]
        private string _outputFolderPath = string.Empty;

        private SerializedObject _serializedObject = null;


        [MenuItem("AkaneTools/Create Block")]
        private static void Open()
        {
            GetWindow<CreateBlockWindow>();
        }

        private void OnEnable()
        {
            _serializedObject = new(this);
        }

        private void OnGUI()
        {
            Properties();

            if (GUILayout.Button("ブロックPrefabを作成する"))
            {
                if (!CanCreate()) { return; }

                CreateBlock();
            }
        }

        private void Properties()
        {
            _serializedObject.Update();

            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_baseObject)));

            EditorGUILayout.Space(10);

            _scale = EditorGUILayout.Vector3Field($"スケール 最小値は{MINIMUM_SCALE_VALUE} 最大値は{MAXIMUM_SCALE_VALUE}", _scale);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("割り当てるマテリアル");
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_material)));

            EditorGUILayout.Space(10);

            _prefixName = EditorGUILayout.TextField("接頭辞", _prefixName);

            EditorGUILayout.Space(10);

            _outputFolderPath = EditorGUILayout.TextField("出力先のフォルダパス", _outputFolderPath);

            EditorGUILayout.Space(10);

            _serializedObject.ApplyModifiedProperties();
        }

        private bool CanCreate()
        {
            if (_baseObject == null)
            {
                Debug.LogWarning("BaseObjectを選択してください！");
                return false;
            }

            if (_scale.x < MINIMUM_SCALE_VALUE || _scale.y < MINIMUM_SCALE_VALUE || _scale.z < MINIMUM_SCALE_VALUE)
            {
                Debug.LogWarning($"スケール値が 最小値{MINIMUM_SCALE_VALUE} 未満になっています！");
                return false;
            }

            if (_scale.x > MAXIMUM_SCALE_VALUE || _scale.y > MAXIMUM_SCALE_VALUE || _scale.z > MAXIMUM_SCALE_VALUE)
            {
                Debug.LogWarning($"スケール値が 最大値{MAXIMUM_SCALE_VALUE} 超過になっています！");
                return false;
            }

            if (_material == null)
            {
                Debug.LogWarning($"マテリアルをセットしてください！");
                return false;
            }

            if (string.IsNullOrEmpty(_outputFolderPath))
            {
                Debug.LogWarning($"出力先のフォルダパスを指定してください！");
                return false;
            }

            if (string.IsNullOrEmpty(_prefixName))
            {
                Debug.LogWarning($"接頭辞は必ずつけてください！");
                return false;
            }

            return true;
        }

        private void CreateBlock()
        {
            //フォルダが存在しない場合は新規に作成
            if (!Directory.Exists(_outputFolderPath))
            {
                Debug.Log("フォルダを作成しました");
                Directory.CreateDirectory(_outputFolderPath);

                AssetDatabase.ImportAsset(_outputFolderPath);
            }

            //例 : block_5_1p1_3 -> 5x1.1x3の大きさのブロック
            var blockName = $"{_prefixName}_{Format4Filename(_scale.x)}_{Format4Filename(_scale.y)}_{Format4Filename(_scale.z)}";
            //作成したブロックへのファイルパス
            var blockFilePath = $"{_outputFolderPath}/{blockName}.prefab";

            //名前が被ったときに、自動的に番号を追加する
            //hoge.png, hoge 1.png, hoge 2.png <<これ
            blockName = AssetDatabase.GenerateUniqueAssetPath(blockName);

            //ブロックの作成
            var block = Instantiate(_baseObject);
            block.transform.localScale = _scale;
            block.name = blockName;
            //マテリアルの参照をセットする
            block.GetComponent<MeshRenderer>().sharedMaterial = _material;

            //Prefabとして保存する
            PrefabUtility.SaveAsPrefabAsset(block, blockFilePath, out bool success);

            if (success) { Debug.Log("Prefabを保存しました"); }
            else
            {
                Debug.LogWarning($"Prefabの保存に失敗しました: {success}");
            }

            //ブロックが存在する場合は破棄する
            if (block != null)
            {
                DestroyImmediate(block);
            }
        }

        private string Format4Filename(float value)
        {
            //小数点をpに置き換えて、小数点第2位まで表示する
            var result = value.ToString("F").Replace(".", "p");

            //小数点が以下が0なら、小数点以下を表す文字列を消す
            if (result.EndsWith("p00"))
            {
                result = result.Substring(0, result.Length - 3);
            }

            return result;
        }

        [MenuItem("CONTEXT/Transform/Get Scale for Create Block")]
        private static void TeleportToTarget(MenuCommand menuCommand)
        {
            //ウィンドウが開いていなければ何もしない
            if (!HasOpenInstances<CreateBlockWindow>())
            {
                Debug.LogWarning("このコマンドを使用する場合は AkaneTools/Create Block を開く必要があります！");
                return;
            }

            var window = GetWindow<CreateBlockWindow>();

            //menuCommandから実行したTransformを取得
            var transform = menuCommand.context as Transform;
            //targetのpositionを選択したオブジェクトのpositionにする
            window._scale = transform.localScale;
        }
    }

}