using UnityEngine;

public enum SupplierType
{
FireRateBoost,
// Add more reward types here
}

[System.Serializable]
public class SupplierData
{
public string supplierName;
public SupplierType type;
}