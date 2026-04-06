using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public Camera mMainCamera;
    public GameObject mChess;
    public GameObject mChessPlant;
    public Text mText;

    private Animator mainCameraAnimtor;
    private bool cameraPlayFinish = false;
    private bool isGeneratingChess = false; // 添加生成状态标志
    private bool isGenerationComplete = false; // 添加生成完成标志
    private bool isRolling = false; // 添加骰子滚动状态标志
    private int currentRedIndex = 0; // 当前显示Red的棋子索引
    private List<GameObject> generatedChessList = new List<GameObject>(); // 存储生成的mChess对象
    
    // 添加UI相关字段
    private Button rollButton;
    private Text numberText;
    private GameObject numberDisplay;
    
    // 添加动画状态检测
    private string currentState = "";
    
    // Start is called before the first frame update
    void Start()
    {
        if (mMainCamera != null)
        {
            mainCameraAnimtor = mMainCamera.GetComponent<Animator>();
        }

        if (mText != null) { mText.enabled = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCameraAnimtor != null)
        {
            // 检测动画是否播放完成
            CheckAnimationCompletion();
        }

        if(cameraPlayFinish && !isGeneratingChess)
        {
            // 开始生成棋子
            StartCoroutine(GenerateChessAroundPlant());
            isGeneratingChess = true;
        }
    }

    public void CreateChess()
    { 

    }
    
    // 添加检测动画完成的方法
    private void CheckAnimationCompletion()
    {
        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = mainCameraAnimtor.GetCurrentAnimatorStateInfo(0);
        
        // 如果动画正在播放且normalizedTime >= 1表示播放完成
        if (stateInfo.normalizedTime >= 1.0f && !cameraPlayFinish)
        {
            cameraPlayFinish = true;
            Debug.Log("Main Camera动画播放完成，cameraPlayFinish设置为true");
        }
    }

    // 添加围绕mChessPlant生成mChess的协程
    private IEnumerator GenerateChessAroundPlant()
    {
        if (mChessPlant == null || mChess == null)
        {
            Debug.LogError("mChessPlant或mChess未设置");
            yield break;
        }

        Vector3 plantSize = Vector3.one * 10.0f;
        Vector3 chessSize = Vector3.one * 0.9f;
        Vector3 plantPosition = mChessPlant.transform.position;

        Debug.Log($"mChessPlant尺寸: {plantSize}, mChess尺寸: {chessSize}");

        // 计算长方形边界
        float plantHalfWidth = plantSize.x / 2;
        float plantHalfLength = plantSize.z / 2;
        float chessHalfWidth = chessSize.x / 2;
        float chessHalfLength = chessSize.z / 2;

        // 计算逆时针排列的位置序列
        List<Vector3> spawnPositions = new List<Vector3>();

        // 底部边（从左到右）
        for (float x = -plantHalfWidth + chessHalfWidth; x <= plantHalfWidth - chessHalfWidth - chessHalfWidth; x += chessSize.x + 0.1f)
        {
            spawnPositions.Add(new Vector3(x, 0, -plantHalfLength + chessHalfLength));
        }

        // 右侧边（从下到上）
        for (float z = -plantHalfLength + chessHalfLength; z <= plantHalfLength - chessHalfLength - chessHalfWidth;  z += chessSize.z + 0.1f)
        {
            spawnPositions.Add(new Vector3(plantHalfWidth - chessHalfWidth, 0, z));
        }

        // 顶部边（从右到左）
        for (float x = plantHalfWidth - chessHalfWidth; x >= -plantHalfWidth + chessHalfWidth + chessHalfWidth; x -= chessSize.x + 0.1f)
        {
            spawnPositions.Add(new Vector3(x, 0, plantHalfLength - chessHalfLength));
        }

        // 左侧边（从上到下）
        for (float z = plantHalfLength - chessHalfLength; z >= -plantHalfLength + chessHalfLength + chessHalfLength; z -= chessSize.z + 0.1f)
        {
            spawnPositions.Add(new Vector3(-plantHalfWidth + chessHalfWidth, 0, z));
        }

        Debug.Log($"开始围绕mChessPlant生成{spawnPositions.Count}个mChess对象，间隔0.5秒");

        // 逆时针生成mChess对象
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            Vector3 worldPosition = plantPosition + spawnPositions[i];
            GameObject newChess = Instantiate(mChess, worldPosition, Quaternion.identity);
            generatedChessList.Add(newChess); // 添加到列表
            Debug.Log($"在第{i+1}个位置生成mChess: {worldPosition}, 已添加到列表，当前总数: {generatedChessList.Count}");
            
            // 设置初始颜色状态：左下角第一个显示Red，其他显示Blue
            SetChessColor(newChess, i == 0);
            
            // 等待0.5秒
            yield return new WaitForSeconds(0.05f);
           
        }

        Debug.Log("mChess生成完成");
        
        // 生成完成后创建按钮
        isGenerationComplete = true;
        mText.enabled = true;

    }


    public void RollButtonClicked()
    {
        if (!isGenerationComplete || isRolling)
        {
            return;
        }
        Debug.Log("RollButtonClicked");
        StartCoroutine(RollDiceAnimation());
    }

    // 掷骰子动画协程
    private IEnumerator RollDiceAnimation()
    {
        if (mText == null)
        {
            Debug.LogError("mText未设置");
            yield break;
        }

        // 设置滚动状态
        isRolling = true;

        // 确保文本可见
        mText.enabled = true;
        
        // 随机显示数字动画（持续约1秒）
        float duration = 1.0f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // 生成1-6的随机数
            int randomNum = Random.Range(1, 7);
            mText.text = randomNum.ToString();
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确定最终数字
        int finalNumber = Random.Range(1, 7);
        mText.text = finalNumber.ToString();
        
        Debug.Log($"最终骰子数字: {finalNumber}");
        
        // 执行颜色变化动画
        yield return StartCoroutine(AnimateColorChange(finalNumber));
        
        // 等待1秒后才能再次点击
        yield return new WaitForSeconds(0.1f);
        
        // 重置滚动状态
        isRolling = false;
    }

    // 设置棋子颜色状态
    private void SetChessColor(GameObject chess, bool showRed)
    {
        if (chess == null) return;
        
        // 查找Cube子对象
        Transform cubeTransform = chess.transform.Find("Cube");
        if (cubeTransform == null) return;
        
        // 查找Blue和Red子对象
        Transform blueTransform = cubeTransform.Find("Blue");
        Transform redTransform = cubeTransform.Find("Red");
        
        if (blueTransform != null)
        {
            blueTransform.gameObject.SetActive(!showRed);
        }
        
        if (redTransform != null)
        {
            redTransform.gameObject.SetActive(showRed);
        }
    }

    // 颜色变化动画协程
    private IEnumerator AnimateColorChange(int steps)
    {
        if (generatedChessList.Count == 0) yield break;
        
        // 动画持续时间
        float animationDuration = 1.0f;
        float stepDuration = animationDuration / steps;
        
        int startIndex = currentRedIndex;
        
        // 首先隐藏所有红色棋子
        foreach (GameObject chess in generatedChessList)
        {
            SetChessColor(chess, false);
        }
        
        // 显示起始位置的红色棋子
        SetChessColor(generatedChessList[startIndex], true);
        
        // 逐步移动红色棋子
        for (int step = 1; step <= steps; step++)
        {
            // 计算下一个位置（逆时针方向）
            int nextIndex = (startIndex + 1) % generatedChessList.Count;
            
            // 隐藏当前红色棋子
            SetChessColor(generatedChessList[startIndex], false);
            
            // 显示下一个红色棋子
            SetChessColor(generatedChessList[nextIndex], true);
            
            // 更新当前红色索引
            currentRedIndex = nextIndex;
            startIndex = nextIndex;
            
            // 等待一段时间
            yield return new WaitForSeconds(0.2f);
        }
        
        Debug.Log($"颜色变化完成，当前红色棋子索引: {currentRedIndex}");
    }
}