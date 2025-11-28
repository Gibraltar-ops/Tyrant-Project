using UnityEngine;

public class AutoDestroyExplosion : MonoBehaviour
{
    Animator m_Animator;
    AnimatorClipInfo[] m_CurrentClipInfo;
    float m_CurrentClipLength;

    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        m_CurrentClipInfo = this.m_Animator.GetCurrentAnimatorClipInfo(0);
        m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        Destroy(gameObject, m_CurrentClipLength);
    }
}
