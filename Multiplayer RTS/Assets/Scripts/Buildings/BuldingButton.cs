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
    private BoxCollider buldingCollider;
    private RTSPlayer player;
    private GameObject buldingPreviewInstance;
    private Renderer buldingRenderInstance;

    private void Start()
    {
        mainCamera = Camera.main;

        iconImage.sprite = bulding.GetICon();
        priceText.text = bulding.GetPrice().ToString();

        buldingCollider = bulding.GetComponent<BoxCollider>();
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

        //if (player.GetResources() < bulding.GetPrice()) { return; } Mudança minha

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
            player.CmdTryPlaceBulding(bulding.GetID(), hit.point);
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

        //Color color = player.CanPlaceBulding(buldingCollider, hit.point) ? Color.green : Color.red; Original

        Color color;

        if(!player.CanPlaceBulding(buldingCollider, hit.point))  //Adicionado por mim.
            color = Color.red;
        else if(player.GetResources() < bulding.GetPrice())
            color = Color.magenta;
        else
            color = Color.green;;

        buldingRenderInstance.material.SetColor("_BaseColor", color);

    }

}
