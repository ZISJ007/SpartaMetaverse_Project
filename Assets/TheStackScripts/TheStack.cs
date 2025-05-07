using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;


public class TheStack : MonoBehaviour
{
    
    private const float BoundSize = 3.5f;         // 블록 최대 크기
    private const float MovingBoundsSize = 3f;    // 블록 이동 범위 배율
    private const float StackMovingSpeed = 5.0f;  // 스택 이동 속도
    private const float BlockMovingSpeed = 3.5f;  // 블록 이동 속도
    private const float ErrorMargin = 0.1f;       // 블록 쌓기 허용 오차

    
    [Tooltip("스택에 사용할 원본 블록 프리팹")] public GameObject originBlock = null;

    
    private Vector3 prevBlockPosition;           // 이전 블록의 위치
    private Vector3 desiredPosition;             // 카메라 이동 목표 위치
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize);  // 현재 블록 크기 (x, z)

    Transform lastBlock = null;                  // 마지막으로 생성된 블록 참조
    float blockTransition = 0f;                  // 블록 이동 애니메이션 타이머
    float secondaryPosition = 0f;                // 보조 축의 위치 저장값

    int stackCount = -1;                         // 쌓인 블록 수
    public int Score { get { return stackCount; } }
    int comboCount = 0;                         
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0;                   
    public int MaxCombo { get => maxCombo; }

    int bestScore = 0;                          
    public int BestScore { get => bestScore; }
    int bestCombo = 0;                           
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";  
    private const string BestComboKey = "BestCombo";

    bool isGameOver = true;                      

   
    public Color preColor;                        // 이전 블록 색상
    public Color nextColor;                       // 다음 목표 색상

    bool isMovingX = true;                       // 이동 축 결정 (X축 or Z축)

  
    void Start()
    {
        if (originBlock == null)
        {
            Debug.LogError("OriginBlock is NULL");
            return;
        }
        // 최고 기록 불러오기
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);
        // 랜덤 색상 초기화
        preColor = GetRandomColor();
        nextColor = GetRandomColor();
        prevBlockPosition = Vector3.down; // 첫 블록 위치 기준

        // 시작할 때 두 개의 블록 생성
        Spawn_Block();
        Spawn_Block();
    }


    void Update()
    {
        if (isGameOver) return; // 게임 오버 시 작동 중지


        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceBlock())  
                Spawn_Block();
            else 
            {
                Debug.Log("Game Over");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
                PlayerPrefs.SetInt("LastScore", stackCount);
                PlayerPrefs.SetInt("LastCombo", comboCount);
                PlayerPrefs.Save();
                UiManager.Instance.SetScoreUI();
            }
        }

        
        MoveBlock();
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
    }

    
    bool Spawn_Block()
    {
        // 이전 블록 위치 저장
        if (lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        // 새 블록 인스턴스 생성
        GameObject newBlock = Instantiate(originBlock);
        if (newBlock == null)
        {
            Debug.LogError("NewBlock Instantiate Failed!");
            return false;
        }

        // 색상 적용
        ColorChange(newBlock);

        // 계층 및 위치 설정
        Transform newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity;
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        // 점수 및 상태 갱신
        stackCount++;
        desiredPosition = Vector3.down * stackCount;
        blockTransition = 0f;
        lastBlock = newTrans;
        isMovingX = !isMovingX;               // 이동 축 전환

        UiManager.Instance.UpdateScore();    // UI 점수 표시 업데이트
        return true;
    }

    
    Color GetRandomColor()
    {
        float r = Random.Range(100f, 250f) / 255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;
        return new Color(r, g, b);
    }

    
    void ColorChange(GameObject go)
    {
        // 이전 색상과 다음 색상 사이 보간
        Color applyColor = Color.Lerp(preColor, nextColor, (stackCount % 11) / 10f);
        Renderer rn = go.GetComponent<Renderer>();
        if (rn == null) Debug.LogError("Renderer is null");
        rn.material.color = applyColor;

        // 배경 색상도 변경
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        // 목표 색상 도달 시 다음 색상 갱신
        if (applyColor.Equals(nextColor))
        {
            preColor = nextColor;
            nextColor = GetRandomColor();
        }
    }

    
    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;
        float movePos = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 2;

        if (isMovingX)
            lastBlock.localPosition = new Vector3(movePos * MovingBoundsSize, stackCount, secondaryPosition);
        else
            lastBlock.localPosition = new Vector3(secondaryPosition, stackCount, -movePos * MovingBoundsSize);
    }

    
    bool PlaceBlock()
    {
        Vector3 lastPos = lastBlock.localPosition;

        if (isMovingX)
        {
            // X축 기준 정렬
            float deltaX = prevBlockPosition.x - lastPos.x;
            bool isNeg = deltaX < 0;
            deltaX = Mathf.Abs(deltaX);

            if (deltaX > ErrorMargin)
            {
                // 잘린 부분 계산 및 블록 크기 조정
                stackBounds.x -= deltaX;
                if (stackBounds.x <= 0) return false;
                float mid = (prevBlockPosition.x + lastPos.x) / 2;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                lastBlock.localPosition = new Vector3(mid, lastPos.y, lastPos.z);

                // 잘린 잔해 생성
                float rubbleHalf = deltaX / 2f;
                CreateRubble(
                    new Vector3(
                        isNeg ? lastPos.x + stackBounds.x / 2 + rubbleHalf : lastPos.x - stackBounds.x / 2 - rubbleHalf,
                        lastPos.y,
                        lastPos.z),
                    new Vector3(deltaX, 1, stackBounds.y)
                );
                comboCount = 0;
            }
            else
            {
                // 거의 정렬된 경우 콤보 증가
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        else
        {
            // Z축 기준 정렬 (위와 동일 로직)
            float deltaZ = prevBlockPosition.z - lastPos.z;
            bool isNeg = deltaZ < 0;
            deltaZ = Mathf.Abs(deltaZ);

            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if (stackBounds.y <= 0) return false;
                float mid = (prevBlockPosition.z + lastPos.z) / 2;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                lastBlock.localPosition = new Vector3(lastPos.x, lastPos.y, mid);

                float rubbleHalf = deltaZ / 2f;
                CreateRubble(
                    new Vector3(
                        lastPos.x,
                        lastPos.y,
                        isNeg ? lastPos.z + stackBounds.y / 2 + rubbleHalf : lastPos.z - stackBounds.y / 2 - rubbleHalf),
                    new Vector3(stackBounds.x, 1, deltaZ)
                );
                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }

        // 보조 위치 저장 및 성공 처리
        secondaryPosition = isMovingX ? lastBlock.localPosition.x : lastBlock.localPosition.z;
        return true;
    }

    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject, this.transform);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;
        go.AddComponent<Rigidbody>(); // 물리 효과
        go.name = "Rubble";
    }

    void ComboCheck()
    {
        comboCount++;
        if (comboCount > maxCombo)
            maxCombo = comboCount;

        // 5콤보 달성 시 블록 크기 증가
        if ((comboCount % 5) == 0)
        {
            Debug.Log("5Combo Success!");
            stackBounds += new Vector3(0.5f, 0.5f);
            stackBounds.x = Mathf.Min(stackBounds.x, BoundSize);
            stackBounds.y = Mathf.Min(stackBounds.y, BoundSize);
        }
    }

    void UpdateScore()
    {
        if (bestScore < stackCount)
        {
            Debug.Log("최고 점수 갱신");
            bestScore = stackCount;
            bestCombo = maxCombo;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = transform.childCount;  // 현재 스택 내 모든 블록(자식) 개수를 가져옴

        // 최근 생성된 20개의 블록(또는 실제 있는 수)만 처리
        for (int i = 1; i < 20 && childCount > i; i++)
        {
            // 뒤에서 i번째 자식(가장 위에 쌓인 블록들부터 차례대로)
            GameObject go = transform.GetChild(childCount - i).gameObject;

            // 이미 잔해(Rubble)로 표시된 블록은 생략
            if (go.name.Equals("Rubble"))
                continue;

            // Rigidbody를 추가하여 물리 법칙을 적용할 준비
            Rigidbody rb = go.AddComponent<Rigidbody>();

            // 위 방향으로 튀어 오르면서 좌우로 흩어지도록 랜덤 힘을 가함
            // Vector3.up * Random.Range(100f,200f): 위로 향하는 힘
            // Vector3.right * Random.Range(-200f,200f): 좌우 흩어지는 힘
            Vector3 upForce = Vector3.up * Random.Range(300f, 400f);
            Vector3 horizontalForce = Vector3.right * Random.Range(-300f, 300f);
            rb.AddForce(upForce + horizontalForce);
        }
    }

    public void Restart()
    {
        // 모든 자식 오브젝트 삭제
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        // 상태 초기화
        isGameOver = false;
        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);
        stackCount = -1;
        isMovingX = true;
        blockTransition = 0f;
        secondaryPosition = 0f;
        comboCount = 0;
        maxCombo = 0;
        prevBlockPosition = Vector3.down;
        preColor = GetRandomColor();
        nextColor = GetRandomColor();

        // 다시 블록 생성
        Spawn_Block();
        Spawn_Block();
    }
}
