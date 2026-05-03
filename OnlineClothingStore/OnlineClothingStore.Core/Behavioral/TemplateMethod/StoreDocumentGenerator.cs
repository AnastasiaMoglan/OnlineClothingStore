namespace OnlineClothingStore.App.Behavioral.TemplateMethod;

public abstract class StoreDocumentGenerator
{
    public string Generate(string documentId)
    {
        string data = LoadData(documentId);

        string document =
            GenerateHeader() +
            Environment.NewLine +
            GenerateContent(data) +
            Environment.NewLine +
            GenerateFooter();

        return Export(document);
    }

    protected abstract string LoadData(string documentId);
    protected abstract string GenerateHeader();
    protected abstract string GenerateContent(string data);
    protected abstract string GenerateFooter();
    protected abstract string Export(string document);
}