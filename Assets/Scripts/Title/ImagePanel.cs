using UnityEngine;
using System.Collections;
using System.IO;


/// <summary>
/// 条件
/// * このGameObject以下に破片用の子GameObject(以下破片オブジェクト)がある
/// * 破片オブジェクトにRigidbodyとColiderが設定されている
/// </summary>
public class ImagePanel : MonoBehaviour
{
	[Tooltip("表示させる画像")]
	public Texture texure = null;

    [Tooltip("表示させる画像")]
    public Texture texure2 = null;

    [Tooltip("破壊前に見える板")]
	public GameObject panel;

    //[Tooltip("破壊前に見える板")]
    //public GameObject panel2;

    [Tooltip("破壊後の破片を纏めているオブジェクト")]
	public GameObject debris;

	public static bool isBreak = false; //debug

    public float timeOut;
    private float timeElapsed;

    private InputManager input_ = null;

    private int PunchCnt;

    private float fInterval;
    private bool bInterval;
    public GameObject ActiveTarget;


    void Start()
	{
        //transform.parent = GameObject.Find("Main Camera").transform;
        //transform.parent = GameObject.Find("Canvas").transform;
        ActiveTarget.SetActive(false);
        bInterval = false;
        fInterval = 0.0f;
        input_ = JoyconManager.Instance.gameObject.GetComponent<InputManager>();
        PunchCnt = 0;

        //エラー処理
        if (texure == null)
		{
			Debug.LogError("Texture is null");
            isBreak = true;
			return;
		}

		debris.SetActive(false); //飛び散らないように非アクティブにしておく

		//Texture設定
		panel.GetComponent<Renderer>().material.mainTexture = texure;

		foreach (Transform child in debris.transform)
		{
			child.gameObject.GetComponent<Renderer>().material.mainTexture = texure; //破片すべてにTexture設定
		} 
	}

	void Update ()
    {

        GameObject.DontDestroyOnLoad(this);

        if (bInterval)
        {
            fInterval += Time.deltaTime;

            if(fInterval >= 0.1f)
            {
                bInterval = false;
                fInterval = 0;
            }
        }

        if (!bInterval && PunchCnt == 0 && input_.GetPunchL() || !bInterval && PunchCnt == 0 && input_.GetPunchR())
        {
            bInterval = true;
            PunchCnt = 1;
            panel.GetComponent<Renderer>().material.mainTexture = texure2;

            Debug.Log(PunchCnt);
        }

        if (!bInterval && PunchCnt == 1 && input_.GetPunchL() || !bInterval && PunchCnt == 1 && input_.GetPunchR())
        {
            bInterval = true;
            isBreak = true;
            crash();
            
            Debug.Log(PunchCnt);
        }
    }

	public void crash()
	{
		debris.SetActive(true); //破片を表示
        debris.transform.parent = null; //Destroy(this.gameObject)に巻き込まれないように
        
        Destroy(debris, 4.0f);

        Destroy(this.gameObject);
	}

    public static bool GetIsBreak()
    {
        return isBreak;
    }
 }
