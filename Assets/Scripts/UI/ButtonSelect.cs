using UnityEngine;
using UnityEngine.UI;

//オブジェクトが有効になったとき、指定したボタンを選択するスクリプト
public class ButtonSelect : MonoBehaviour
{
    [SerializeField]
    private Button button = null;

    private void OnEnable()
    {
        button.Select();
    }
}
