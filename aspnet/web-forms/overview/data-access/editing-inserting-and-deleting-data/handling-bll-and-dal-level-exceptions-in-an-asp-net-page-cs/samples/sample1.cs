[System.ComponentModel.DataObjectMethodAttribute(
    System.ComponentModel.DataObjectMethodType.Update, false)]
public bool UpdateProduct(string productName, decimal? unitPrice, short? unitsInStock,
    int productID)
{
    Northwind.ProductsDataTable products = Adapter.GetProductByProductID(productID);
    if (products.Count == 0)
        // no matching record found, return false
        return false;
    Northwind.ProductsRow product = products[0];
    product.ProductName = productName;
    if (unitPrice == null) product.SetUnitPriceNull();
      else product.UnitPrice = unitPrice.Value;
    if (unitsInStock == null) product.SetUnitsInStockNull();
      else product.UnitsInStock = unitsInStock.Value;
    // Update the product record
    int rowsAffected = Adapter.Update(product);
    // Return true if precisely one row was updated, otherwise false
    return rowsAffected == 1;
}