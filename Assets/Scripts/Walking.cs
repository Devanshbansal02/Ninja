using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;

public class Walking : MonoBehaviour
{
    public float speed = 10f;
    private bool isMoving;
    private Vector2 control;
    private Animator animator;
    public LayerMask solidObjectsLayer;
    public LayerMask npc;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    private void Update()
    {
        if(!isMoving){
        control.x=Input.GetAxisRaw("Horizontal");
        control.y=Input.GetAxisRaw("Vertical");
        if(control.x!=0) control.y=0;

        if(control!=Vector2.zero){
            animator.SetFloat("moveX", control.x);
            animator.SetFloat("moveY", control.y);
            var targetPos = transform.position;
            targetPos.x += control.x;
            targetPos.y += control.y;
            if(isWalkable(targetPos))
            StartCoroutine(Move(targetPos));

        }
        }
        animator.SetBool("IsWalking", isMoving);
        if(Input.GetKeyDown(KeyCode.Z))
        Interact();
    }
    IEnumerator Move(Vector3 targetPos){
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    void Interact(){
        var facedir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactpos = transform.position + facedir;
        Debug.DrawLine(transform.position, interactpos, Color.blue, 1f);
        var collider = Physics2D.OverlapCircle(interactpos, 0.2f, npc);
        if(collider != null){
            // Debug.Log("there is a npc");
            collider.GetComponent<interactable>()?.Interact();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("collided");
    }

    private bool isWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | npc) != null)
        {

            Debug.Log("there is an obs");
            return true;
        }
        return true;
    }
}