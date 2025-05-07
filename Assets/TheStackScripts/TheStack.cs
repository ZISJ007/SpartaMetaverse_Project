using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

// ��� �ױ� ������ �ٽ� ������ ����ϴ� Ŭ����
public class TheStack : MonoBehaviour
{
    
    private const float BoundSize = 3.5f;         // ��� �ִ� ũ��
    private const float MovingBoundsSize = 3f;    // ��� �̵� ���� ����
    private const float StackMovingSpeed = 5.0f;  // ���� �̵� �ӵ�
    private const float BlockMovingSpeed = 3.5f;  // ��� �̵� �ӵ�
    private const float ErrorMargin = 0.1f;       // ��� �ױ� ��� ����

    
    [Tooltip("���ÿ� ����� ���� ��� ������")] public GameObject originBlock = null;

    
    private Vector3 prevBlockPosition;           // ���� ����� ��ġ
    private Vector3 desiredPosition;             // ī�޶� �̵� ��ǥ ��ġ
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize);  // ���� ��� ũ�� (x, z)

    Transform lastBlock = null;                  // ���������� ������ ��� ����
    float blockTransition = 0f;                  // ��� �̵� �ִϸ��̼� Ÿ�̸�
    float secondaryPosition = 0f;                // ���� ���� ��ġ ���尪

    int stackCount = -1;                         // ���� ��� ��
    public int Score { get { return stackCount; } }
    int comboCount = 0;                          // ���� �޺� ��
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0;                    // �ְ� �޺� ��
    public int MaxCombo { get => maxCombo; }

    int bestScore = 0;                           // �ְ� ����
    public int BestScore { get => bestScore; }
    int bestCombo = 0;                           // �ְ� �޺�
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";  // PlayerPrefs Ű
    private const string BestComboKey = "BestCombo";

    bool isGameOver = true;                      // ���� ���� ����

   
    public Color preColor;                        // ���� ��� ����
    public Color nextColor;                       // ���� ��ǥ ����

    bool isMovingX = true;                       // �̵� �� ���� (X�� or Z��)

  
    void Start()
    {
        if (originBlock == null)
        {
            Debug.LogError("OriginBlock is NULL");
            return;
        }
        // �ְ� ��� �ҷ�����
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);
        // ���� ���� �ʱ�ȭ
        preColor = GetRandomColor();
        nextColor = GetRandomColor();
        prevBlockPosition = Vector3.down; // ù ��� ��ġ ����

        // ������ �� �� ���� ��� ����
        Spawn_Block();
        Spawn_Block();
    }


    void Update()
    {
        if (isGameOver) return; // ���� ���� �� �۵� ����


        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceBlock())  
                Spawn_Block();
            else //���� ����
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

        // ��� �̵� �� ī�޶� �̵�
        MoveBlock();
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
    }

    // ----------------------------- ��� ���� -----------------------------
    bool Spawn_Block()
    {
        // ���� ��� ��ġ ����
        if (lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        // �� ��� �ν��Ͻ� ����
        GameObject newBlock = Instantiate(originBlock);
        if (newBlock == null)
        {
            Debug.LogError("NewBlock Instantiate Failed!");
            return false;
        }

        // ���� ����
        ColorChange(newBlock);

        // ���� �� ��ġ ����
        Transform newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity;
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        // ���� �� ���� ����
        stackCount++;
        desiredPosition = Vector3.down * stackCount;
        blockTransition = 0f;
        lastBlock = newTrans;
        isMovingX = !isMovingX;               // �̵� �� ��ȯ

        UiManager.Instance.UpdateScore();    // UI ���� ǥ�� ������Ʈ
        return true;
    }

    // ----------------------------- ���� ���� ���� -----------------------------
    Color GetRandomColor()
    {
        float r = Random.Range(100f, 250f) / 255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;
        return new Color(r, g, b);
    }

    // ----------------------------- ���� ���� -----------------------------
    void ColorChange(GameObject go)
    {
        // ���� ����� ���� ���� ���� ����
        Color applyColor = Color.Lerp(preColor, nextColor, (stackCount % 11) / 10f);
        Renderer rn = go.GetComponent<Renderer>();
        if (rn == null) Debug.LogError("Renderer is null");
        rn.material.color = applyColor;

        // ��� ���� ����
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        // ��ǥ ���� ���� �� ���� ���� ����
        if (applyColor.Equals(nextColor))
        {
            preColor = nextColor;
            nextColor = GetRandomColor();
        }
    }

    // ----------------------------- ��� �̵� -----------------------------
    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;
        float movePos = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 2;

        if (isMovingX)
            lastBlock.localPosition = new Vector3(movePos * MovingBoundsSize, stackCount, secondaryPosition);
        else
            lastBlock.localPosition = new Vector3(secondaryPosition, stackCount, -movePos * MovingBoundsSize);
    }

    // ----------------------------- ��� ���� �Ǵ� -----------------------------
    bool PlaceBlock()
    {
        Vector3 lastPos = lastBlock.localPosition;

        if (isMovingX)
        {
            // X�� ���� ����
            float deltaX = prevBlockPosition.x - lastPos.x;
            bool isNeg = deltaX < 0;
            deltaX = Mathf.Abs(deltaX);

            if (deltaX > ErrorMargin)
            {
                // �߸� �κ� ��� �� ��� ũ�� ����
                stackBounds.x -= deltaX;
                if (stackBounds.x <= 0) return false;
                float mid = (prevBlockPosition.x + lastPos.x) / 2;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                lastBlock.localPosition = new Vector3(mid, lastPos.y, lastPos.z);

                // �߸� ���� ����
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
                // ���� ���ĵ� ��� �޺� ����
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        else
        {
            // Z�� ���� ���� (���� ���� ����)
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

        // ���� ��ġ ���� �� ���� ó��
        secondaryPosition = isMovingX ? lastBlock.localPosition.x : lastBlock.localPosition.z;
        return true;
    }

    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject, this.transform);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;
        go.AddComponent<Rigidbody>(); // ���� ȿ��
        go.name = "Rubble";
    }

    void ComboCheck()
    {
        comboCount++;
        if (comboCount > maxCombo)
            maxCombo = comboCount;

        // 5�޺� �޼� �� ��� ũ�� ����
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
            Debug.Log("�ְ� ���� ����");
            bestScore = stackCount;
            bestCombo = maxCombo;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = transform.childCount;  // ���� ���� �� ��� ���(�ڽ�) ������ ������

        // �ֱ� ������ 20���� ���(�Ǵ� ���� �ִ� ��)�� ó��
        for (int i = 1; i < 20 && childCount > i; i++)
        {
            // �ڿ��� i��° �ڽ�(���� ���� ���� ��ϵ���� ���ʴ��)
            GameObject go = transform.GetChild(childCount - i).gameObject;

            // �̹� ����(Rubble)�� ǥ�õ� ����� ����
            if (go.name.Equals("Rubble"))
                continue;

            // Rigidbody�� �߰��Ͽ� ���� ��Ģ�� ������ �غ�
            Rigidbody rb = go.AddComponent<Rigidbody>();

            // �� �������� Ƣ�� �����鼭 �¿�� ��������� ���� ���� ����
            // Vector3.up * Random.Range(100f,200f): ���� ���ϴ� ��
            // Vector3.right * Random.Range(-200f,200f): �¿� ������� ��
            Vector3 upForce = Vector3.up * Random.Range(300f, 400f);
            Vector3 horizontalForce = Vector3.right * Random.Range(-300f, 300f);
            rb.AddForce(upForce + horizontalForce);
        }
    }

    public void Restart()
    {
        // ��� �ڽ� ������Ʈ ����
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        // ���� �ʱ�ȭ
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

        // �ٽ� ��� ����
        Spawn_Block();
        Spawn_Block();
    }
}
