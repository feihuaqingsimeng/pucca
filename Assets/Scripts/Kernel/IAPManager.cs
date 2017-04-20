using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using UnityEngine.Purchasing.Security;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    #region Variables
    IStoreController m_StoreController; // The Unity Purchasing system.
    IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    #endregion

    #region Properties
    public bool purchaseProcessing
    {
        get;
        /*private*/
        set;
    }

    public int purchaseProductIndex
    {
        get;
        private set;
    }
    #endregion

    #region Delegates
    public delegate void OnPurchaseResult(bool purchaseProcessing, Product product, PurchaseFailureReason failureReason);
    public OnPurchaseResult onPurchaseResult;
    #endregion

    #region MonoBehaviour
    // Use this for initialization
    /*
    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }
    */
    // Update is called once per frame
    #endregion

    #region
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        if (Application.platform == RuntimePlatform.Android)
        {
            builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAorepHq4X4Zfxm2D3qT2at3gpXkxXBuDwWsAXrELcIdUKGP+/PhD23emvXD0YTRVlQ7I5gHJg4jPAMjacAFh03DAr1rxGL69O7atr7zT4Dnw0JLuECftEVJL317/DjMligZ16TeB7mHfwHiMdCbKgeRgMqHTkQgLQrpqfUxf//Hz8HTjLfbHn3p4uzO12cCPpJTI5eOcFyi23otowS1Q5XI58sQTaHxMkGo1ERWGCl9z4Y6X9oUU8ShfXH01jNAsJDsezxtjbMN2io3NUTBukhvQeVCIcRgUcIkbomRNau+oHIaVG2NttmYgXTbXOLn0fICo+VswGahKFapqF4zy32wIDAQAB");
        }

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        for (int i = 0; i < DB_ProductMain.instance.schemaList.Count; i++)
        {
            DB_ProductMain.Schema productMain = DB_ProductMain.instance.schemaList[i];
            string productId = GetProductId(productMain);
            if (!string.IsNullOrEmpty(productId))
            {
                builder.AddProduct(productId, ProductType.Consumable);
            }
        }

        // Continue adding the non-consumable product.

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores.

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    public void Purchase(int productIndex)
    {
        Debug.Log("[IAPManager] : Purchase : purchaseProcessing = " + purchaseProcessing);

        if (purchaseProcessing)
        {
            purchaseProcessing = false;
        }

        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            DB_ProductMain.Schema productMain = DB_ProductMain.Query(DB_ProductMain.Field.Index, productIndex);
            if (productMain != null)
            {
                string productId = GetProductId(productMain);
                if (!string.IsNullOrEmpty(productId))
                {
                    // ... look up the Product reference with the general product identifier and the Purchasing
                    // system's products collection.
                    Product product = m_StoreController.products.WithID(productId);
                    // If the look up found a product for this device's store and that product is ready to be sold ...
                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                        // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed
                        // asynchronously.
                        purchaseProductIndex = productIndex;
                        purchaseProcessing = true;
                        m_StoreController.InitiatePurchase(product);
                    }
                }
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public Product FindProduct(int productIndex)
    {
        if (IsInitialized())
        {
            DB_ProductMain.Schema productMain = DB_ProductMain.Query(DB_ProductMain.Field.Index, productIndex);
            if (productMain != null)
            {
                string productId = GetProductId(productMain);
                if (!string.IsNullOrEmpty(productId))
                {
                    return m_StoreController.products.WithID(productId);
                }
            }
        }

        return null;
    }

    public Product FindProduct(string productId)
    {
        return IsInitialized() ? m_StoreController.products.WithID(productId) : null;
    }

    public void ConfirmPendingPurchase(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null)
            {
                purchaseProcessing = false;
                purchaseProductIndex = 0;
                m_StoreController.ConfirmPendingPurchase(product);

                Debug.Log(string.Format("[LOG] ConfirmPendingPurchase productid : {0}", productId));
            }
            else
            {
                Debug.Log(string.Format("[ERROR] ConfirmPendingPurchase Invalid productid : {0}", productId));
            }

            /*
            DB_ProductMain.Schema productMain = DB_ProductMain.Query(DB_ProductMain.Field.Index, purchaseProductIndex);
            if (productMain != null)
            {
                string productId = GetProductId(productMain);
                if (!string.IsNullOrEmpty(productId))
                {
                    Product product = m_StoreController.products.WithID(productId);
                    if (product != null)
                    {
                        purchaseProcessing = false;
                        purchaseProductIndex = 0;
                        m_StoreController.ConfirmPendingPurchase(product);
                    }
                }
            }
            */
        }
        else
        {
            Debug.Log("[ERROR] ConfirmPendingPurchase not initialized - productid : " + productId);
        }
    }
    #endregion

    bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return (m_StoreController != null) && (m_StoreExtensionProvider != null);
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ...
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public static string GetProductId(DB_ProductMain.Schema productMain)
    {
        string productId = string.Empty;
        if (productMain != null)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    productId = productMain.PlayStoreProductId;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    productId = productMain.AppStoreProductId;
                    break;
                default:
                    Debug.LogWarningFormat("{0} is not supported platform.", Application.platform);
                    break;
            }
        }

        return productId;
    }

    #region IStoreListener
    /// <summary>
    /// Called when Unity IAP has retrieved all product metadata and is ready to
    /// make purchases.
    /// </summary>
    /// <param name="controller">
    /// Access cross-platform Unity IAP functionality.
    /// </param>
    /// <param name="extensions">
    /// Access store-specific Unity IAP functionality.
    /// </param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        foreach (var product in controller.products.all)
        {
            Debug.LogFormat("{0}\nstoreSpecificId : {1}\nisoCurrencyCode : {2}\nlocalizedDescription : {3}\nlocalizedPrice : {4}\nlocalizedPriceString : {5}\nlocalizedTitle : {6}",
                            product.definition.id,
                            product.definition.storeSpecificId,
                            product.metadata.isoCurrencyCode,
                            product.metadata.localizedDescription,
                            product.metadata.localizedPrice,
                            product.metadata.localizedPriceString,
                            product.metadata.localizedTitle);
        }
    }

    /// <summary>
    /// Note that Unity IAP will not call this method if the device is offline,
    /// but continually attempt initialization until online.
    /// </summary>
    /// <param name="error">
    /// The reason IAP cannot initialize.
    /// </param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    /// <param name="i">
    /// The product the purchase relates to.
    /// </param>
    /// <param name="p">
    /// The reason for the failure.
    /// </param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
        // this reason with the user to guide their troubleshooting actions.
        purchaseProcessing = false;
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        if (onPurchaseResult != null)
        {
            onPurchaseResult(purchaseProcessing, product, failureReason);
        }
    }

    /// <summary>
    /// Called when a purchase succeeds.
    /// </summary>
    /// <param name="e">
    /// The purchase details.
    /// </param>
    /// <returns>
    /// Applications should only return PurchaseProcessingResult.Complete when a
    /// permanent record of the purchase has been made. If PurchaseProcessingResult.Pending
    /// is returned Unity IAP will continue to notify the app of the purchase on
    /// startup, also via ProcessPurchase.
    /// </returns>
    /// 

    

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // Return a flag indicating whether this product has completely been received, or if the application needs
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
        // saving purchased products to the cloud, and when that save is delayed.

        Product purchasedProduct = args.purchasedProduct;
        if (purchasedProduct.hasReceipt)
        {
            string productId = purchasedProduct.definition.id;
            DB_ProductMain.Schema productMain = null;

            // Debug.Log(string.Format("[LOG] ProcessPurchase - productid : {0}", productId));

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    productMain = DB_ProductMain.Query(DB_ProductMain.Field.PlayStoreProductId, productId);
                    if (productMain != null)
                    {
                        string receipt = purchasedProduct.receipt;
                        Kernel.entry.billing.REQ_PACKET_CG_BILLING_BUY_ITEM_GOOGLE_SYN(productMain.Index,
                                                                                       productId,
                                                                                       productMain.PackageId,
                                                                                       receipt);

                        if (onPurchaseResult != null)
                        {
                            onPurchaseResult(purchaseProcessing, purchasedProduct, 0);
                        }
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    productMain = DB_ProductMain.Query(DB_ProductMain.Field.AppStoreProductId, productId);
                    if (productMain != null)
                    {
                        string receipt = purchasedProduct.receipt;
                        Kernel.entry.billing.REQ_PACKET_CG_BILLING_BUY_ITEM_APPLE_SYN(productMain.Index,
                                                                                       productId,
                                                                                       productMain.PackageId,
                                                                                       receipt);

                        if (onPurchaseResult != null)
                        {
                            onPurchaseResult(purchaseProcessing, purchasedProduct, 0);
                        }
                    }
                    break;
            }
        }

        return PurchaseProcessingResult.Pending;
    }
    #endregion

    #region Deprecated
    /*
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.

        // Continue adding the non-consumable product.

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores.

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ...
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }
    */
    #endregion
}
