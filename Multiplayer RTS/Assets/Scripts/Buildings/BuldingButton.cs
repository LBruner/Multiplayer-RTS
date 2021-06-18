using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuldingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    [SerializeField] private Bulding bulding = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TextMeshProUGUI priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera;
    private RTSPlayer player;
    private GameObject buldingPreviewInstance;
    private Renderer buldingRenderInstance;

    private void Start()
    {
        mainCamera = Camera.main;

        iconImage.sprite = bulding.GetICon();
        priceText.text = bulding.GetPrice().ToString();
    }

    private void Update()
    {
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (buldingPreviewInstance == null) { return; }
        
        UpdateBuldingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        buldingPreviewInstance = Instantiate(bulding.GetBuldingPreview());
        buldingRenderInstance = buldingPreviewInstance.GetComponentInChildren<Renderer>();

        //buldingPreviewInstance.SetActive(false); Tava aqui originalmente
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buldingPreviewInstance == null) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {

        }

        Destroy(buldingPreviewInstance);
    }

    private void UpdateBuldingPreview()
    {
        buldingPreviewInstance.SetActive(false); //Ñ tava aqui originalmente

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }

        buldingPreviewInstance.transform.position = hit.point;
        if(!buldingPreviewInstance.activeSelf)
        {
            buldingPreviewInstance.SetActive(true);
        }
    
    }

}
