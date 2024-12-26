using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceTile_ : MonoBehaviour
{
    public TileData tileData;
    public float offset = 0.5f; // �浹ü���� �Ÿ� ������
    public bool isMoveing = false;
    public string word;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = word;
    }

    void Update()
    {
        if (!isMoveing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) StartSlide(Vector2.up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) StartSlide(Vector2.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartSlide(Vector2.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) StartSlide(Vector2.right);
        }
    }

    void StartSlide(Vector2 direction)
    {
        isMoveing = true;
        // Ray ���� ��ġ �� ���� ����
        Ray2D ray = new Ray2D(transform.position, direction);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, tileData.targetLayer);

        if (hits.Length > 1) // �ڱ� �ڽ��� �ݶ��̴��� ������ �浹 Ȯ��
        {
            Vector2 hitPoint = hits[1].point;
            Vector2 adjustedPoint = hitPoint - (direction * offset);

            StartCoroutine(Move(adjustedPoint));
            Debug.DrawLine(transform.position, hitPoint, Color.red, 1f);
        }
        else
        {
            Debug.Log("Ray did not hit any object.");
        }
    }

    IEnumerator Move(Vector2 targetPosition)
    {
        Vector3 startPosition = transform.position; // ���� ��ġ ����
        float distance = Vector3.Distance(startPosition, targetPosition); // ���� ��ġ�� ��ǥ ��ġ �� �Ÿ� ���
        float duration = distance / tileData.speed; // �̵� �Ÿ� ������� ���� �ð� ���

        float elapsedTime = 0f; // ��� �ð� �ʱ�ȭ

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // �ð� ����
            yield return null; // ���� �����ӱ��� ���
        }

        isMoveing = false;
        transform.position = targetPosition; // ���� ��ġ ����
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
        string jungTable = "�������¤äĤŤƤǤˤ̤ФѤӤȤɤʤͤΤϤ�";
        string jongTable = "������������������������������������������������������";

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
