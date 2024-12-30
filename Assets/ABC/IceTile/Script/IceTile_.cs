using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceTile_ : MonoBehaviour
{
    public TileData tileData;
    public float offset = 0.5f; // �浹ü���� �Ÿ� ������
    public float rayOffset = 0.5f; // �浹ü���� �Ÿ� ������
    public bool isMoveing = false;
    public string word;

    private TextMeshProUGUI text;
    private Coroutine slideCoroutine; // ���� ���� �ڷ�ƾ�� ����
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GetComponentInParent<LevelManager>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = word;
    }

    void Update()
    {
        if (!isMoveing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) StartSlideToPhysics(Vector2.up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) StartSlideToPhysics(Vector2.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartSlideToPhysics(Vector2.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) StartSlideToPhysics(Vector2.right);
        }
    }

    void StartSlideToPhysics(Vector2 direction)
    {
        isMoveing = true;
        // �浹 üũ
        if (CheckForCollision(transform.position, direction))
        {
            Debug.Log("Movement stopped due to collision");
            StopSlideCoroutine();
        }
        else
        {
            StartSlideCoroutine(direction);
        }
    }

    bool CheckForCollision(Vector3 position, Vector2 direction)
    {
        Ray2D ray = new Ray2D(position, direction);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, offset, tileData.targetLayer);

        // �浹�� �ִ��� Ȯ�� (�ڱ� �ڽ� ����)
        return hits.Length > 1;
    }

    void StartSlideCoroutine(Vector2 direction)
    {
        // ���� �ڷ�ƾ�� ������ ����
        StopSlideCoroutine();

        // �� �����̵� �ڷ�ƾ ����
        slideCoroutine = StartCoroutine(SlidePhysics(direction));
    }

    void StopSlideCoroutine()
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }
    }

    IEnumerator SlidePhysics(Vector2 direction)
    {
        SoundManager.Instance.PlaySFXMusic("TileMove");
        float stepDistance = 0.1f; // �̵� ���� �Ÿ�

        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + (Vector3)direction * stepDistance;

            // ��ǥ �������� �ε巴�� �̵�
            yield return SmoothMove(startPosition, targetPosition, tileData.speed, direction);

            // ��ǥ �������� �浹 üũ
            transform.position = targetPosition;

            if (CheckForCollision(transform.position, direction))
            {
                Debug.Log("Collision detected, stopping movement");
                isMoveing = false;
                break; // �浹 �� �̵� �ߴ�
            }
        }

        slideCoroutine = null; // �ڷ�ƾ ���� �� �ʱ�ȭ
    }

    IEnumerator SmoothMove(Vector3 start, Vector3 end, float speed, Vector2 direction)
    {
        float elapsedTime = 0f;
        float travelTime = Vector3.Distance(start, end) / speed;

        while (elapsedTime < travelTime)
        {
            if (CheckForCollision(transform.position, direction)) break;

            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsedTime / travelTime);
            yield return null;
        }

        transform.position = end; // �̵� �Ϸ� �� ��Ȯ�� ��ǥ ������ ��ġ
    }

private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.collider.CompareTag("MovableBlock"))
    {
        IceTile_ targetTile = collision.collider.GetComponent<IceTile_>();
        if (targetTile != null)
        {
            string targetWord = targetTile.word;

            // ���� �õ�
            string combinedWord = CombineHangul(word, targetWord);
            if (!string.IsNullOrEmpty(combinedWord))
            {
                    SoundManager.Instance.PlaySFXMusic("TileCombine");

                levelManager.OnBlockCrushed(this, combinedWord);

                // ���� ���� �� ���� Ÿ���� word ����
                word = combinedWord;
                text.text = word;

                // ���յ� Ÿ�� ����
                Destroy(targetTile.gameObject);
            }
            else
            {
                Debug.Log("���� �Ұ���: " + word + " + " + targetWord);
            }
        }
    }
}

    private string CombineHangul(string baseWord, string targetWord)
    {
        if (string.IsNullOrEmpty(baseWord) || string.IsNullOrEmpty(targetWord))
            return null;

        char baseChar = baseWord[baseWord.Length - 1];
        char targetChar = targetWord[0];

        // �ʼ�/�߼�/���� ����
        int baseCode = baseChar - 0xAC00; // �ѱ� �����ڵ� ������
        int targetCode = targetChar - 0xAC00;

        // �ʼ�/�߼�/���� ���̺�
        string choTable = "��������������������������������������";
        string jungTable = "�������¤äĤŤƤǤˤ̤ФѤ�";
        string jongTable = "������������������";

        // ���� ���ڰ� �ʼ�����, �߼����� Ȯ��
        if (choTable.Contains(baseChar) && jungTable.Contains(targetChar))
        {
            // �ʼ� + �߼� ����
            int choIndex = choTable.IndexOf(baseChar);
            int jungIndex = jungTable.IndexOf(targetChar);

            char combinedChar = (char)(0xAC00 + (choIndex * 21 * 28) + (jungIndex * 28));
            return baseWord.Substring(0, baseWord.Length - 1) + combinedChar;
        }
        else if (baseCode >= 0 && baseCode < 11172) // ��ȿ�� ������ ���
        {
            int baseCho = baseCode / (21 * 28); // �ʼ�
            int baseJung = (baseCode % (21 * 28)) / 28; // �߼�
            int baseJong = baseCode % 28; // ����

            if (baseJong == 0 && jongTable.Contains(targetChar))
            {
                // ������ ���� ���, ���� �߰�
                int jongIndex = jongTable.IndexOf(targetChar);
                char combinedChar = (char)(0xAC00 + (baseCho * 21 * 28) + (baseJung * 28) + jongIndex);
                return baseWord.Substring(0, baseWord.Length - 1) + combinedChar;
            }
        }

        return null; // ���� �Ұ���
    }
}
