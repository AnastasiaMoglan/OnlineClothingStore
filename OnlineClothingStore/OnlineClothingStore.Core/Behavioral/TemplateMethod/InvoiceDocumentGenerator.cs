namespace OnlineClothingStore.App.Behavioral.TemplateMethod;

public class InvoiceDocumentGenerator : StoreDocumentGenerator
{
    protected override string LoadData(string documentId)
    {
        return $"Date factură pentru comanda {documentId}";
    }

    protected override string GenerateHeader()
    {
        return "FACTURĂ - Online Clothing Store";
    }

    protected override string GenerateContent(string data)
    {
        return $"Conținut factură: {data}. Include produse, cantități, TVA și total.";
    }

    protected override string GenerateFooter()
    {
        return "Factura a fost generată automat.";
    }

    protected override string Export(string document)
    {
        return $"PDF INVOICE:\n{document}";
    }
}