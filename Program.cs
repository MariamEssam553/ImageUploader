using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
string jsonPath = "uploaded-images.json";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    string html = $@"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <title> Image Uploader </title>
        <meta charset=""utf-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css"" 
            integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" 
            crossorigin=""anonymous"">
        <style>
            .btn-custom{{
                font-weight: 700;
                border: solid #e91e63 3px;
                border-radius: 7px;
                width: fit-content;
                align-self: flex-end; 
                color: #e91e63; 
                background-color:#ffffff00;
                border-radius: 120x; }}  
            .btn-custom:hover{{
                background-color: #e91e63 !important; 
                color:#ffffff !important; 
                border: solid 3px #e91e63 !important; }}
            .container{{
                height: 100vh;
                align-items: center;
                display: flex;
                justify-content: center;}}
            .card{{
                width: 60rem;
                height: 26rem; 
                border-radius: 7px;
                background-color: aliceblue; 
                box-shadow:rgba(0 ,0 ,0 ,0.15) 0px 2px 8px;}}
            .card-header{{
                background-color:#b7d4ed; 
                border-radius: 7px 7px 0px 0px !important;
                font-size: x-large;
                color: #E91E63;
                font-weight: bold;}}
            form{{
                height: 22rem; 
                display: flex; 
                width: 80%;
                align-self: center; }}
            .card-body{{
                display: flex;
                flex-direction: column;
                justify-content: space-evenly}}
        </style>
    </head>
    <body>
    <div class=""container"">
        <div class=""card"">
            <div class=""card-header text-center""> Image Uploader </div>
            <form action=""/upload"" method=""POST"" enctype=""multipart/form-data"">
                <div class=""card-body"">
                    <div class=""form-group"">
                        <input type=""text"" id=""title"" name=""title"" placeholder=""Enter image title"" class=""form-control"" required>
                    </div>
                    <div class=""input-group mb-3"">
                        <div class=""form-group"">
                            <input type=""file"" class=""custom-file-input"" id=""imageFile"" name=""imageFile"" style=""cursor: pointer;"" accept=""image/jpeg, image/png, image/gif"" required>
                            <label class=""custom-file-label"" id=""imageLabel"" for=""imageFile"" style=""cursor: pointer;"">Choose an image</label>
                            <span id=""errorMessage""></span>
                        </div>
                    </div>
                    <button class=""btn btn-custom"" type=""submit"">Submit</button>    
                </div> 
            </form>
        </div>
    </div>
    <script>
        var input = document.getElementById( 'imageFile' );
        var infoArea = document.getElementById( 'imageLabel' );

        function showFileName( event ) {{
            var input = event.srcElement;
            var fileName = input.files[0].name;
            infoArea.textContent = fileName;
         }}

        input.addEventListener( 'change', showFileName );

    </script>
    <script src=""https://code.jquery.com/jquery-3.2.1.slim.min.js"" integrity=""sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"" 
        crossorigin=""anonymous""></script>
    <script src=""https://cdn.jsdelivr.net/npm/popper.js@1.12.9/dist/umd/popper.min.js"" integrity=""sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"" 
        crossorigin=""anonymous""></script>
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/js/bootstrap.min.js"" integrity=""sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl"" 
        crossorigin=""anonymous""></script>
    
    </body>
    </html> ";

    return Results.Text(html, "text/html");
});

app.MapPost("/upload", async (HttpContext context) =>
{
    var formData = await context.Request.ReadFormAsync();

    // Access form fields using the form data collection
    string? imgTitle = formData["title"];
    var imgFile = formData.Files.GetFile("imageFile");
    string imgID = Guid.NewGuid().ToString();

    //Validating file type
    bool IsFileValidType(IFormFile file)
    {
        // Specify the allowed file extensions
        var allowedExtensions = new[] { ".gif", ".jpeg", ".png" };

        // Get the file extension
        var fileExtension = Path.GetExtension(file.FileName);

        // Check if the file extension matches the allowed extensions
        return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    if (!IsFileValidType(imgFile))
    {
        await context.Response.WriteAsync("Invalid file type");
        return;
    }

    //adding to json file
    using (var stream = new MemoryStream())
    {
        await imgFile.CopyToAsync(stream);


        var imgObj = new ImageFile
        {
            id = imgID,
            title = imgTitle,
            url = Convert.ToBase64String(stream.ToArray())
        };

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
        };

        string jsonString = JsonSerializer.Serialize(imgObj, jsonOptions);

        File.WriteAllText(jsonPath, jsonString);
    }

    context.Response.Redirect($"/image/{imgID}");
});

app.MapGet("/image/{id}", async (HttpContext context) =>
{
    string jsonString = await File.ReadAllTextAsync(jsonPath);
    ImageFile? imageObj = JsonSerializer.Deserialize<ImageFile>(jsonString);

    string html = $@"
    <!DOCTYPE html>
     <html>
        <head>
            <title> Image Uploader </title>
            <meta charset=""utf-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
            <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css"" 
              integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" 
              crossorigin=""anonymous"">
            <style>
                .container{{
                    flex-direction: column;
                    height: 100vh;
                    align-items: center;
                    display: flex;
                    justify-content: space-evenly; }}
                .btn-custom{{
                    font-weight: 700;
                    border: solid #e91e63 3px;
                    border-radius: 7px;
                    width: fit-content;
                    align-self: flex-end; 
                    color: #e91e63; 
                    background-color:#ffffff00;
                    border-radius: 120x; }}  
                .btn-custom:hover{{
                    background-color: #e91e63 !important; 
                    color:#ffffff !important; 
                    border: solid 3px #e91e63 !important; }}
                .card-custom{{
                    width: 20rem;
                    border-radius: 7px;
                    background-color: aliceblue; 
                    box-shadow:rgba(0 ,0 ,0 ,0.15) 0px 2px 8px;}}
                .card-text{{
                    text-align: center;
                    font-size: x-large;
                    font-weight: 600;
                    color: #e91e63;}}
            </style>
        </head>
        <body class=""container"">
            <div class=""card card-custom"">
                <img src=""data:image/jpeg;base64,{imageObj.url}"" class=""card-img-top"" alt=""{imageObj.title}"">
                <div class=""card-body"">
                    <p class=""card-text"">{imageObj.title}</p>
                </div>
            </div>
            <a href=""/"">
                <button type=""button"" class=""btn btn-custom"">Back</button>
            </a>
                
            <script src=""https://code.jquery.com/jquery-3.2.1.slim.min.js"" integrity=""sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"" 
                crossorigin=""anonymous""></script>
            <script src=""https://cdn.jsdelivr.net/npm/popper.js@1.12.9/dist/umd/popper.min.js"" integrity=""sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"" 
                 crossorigin=""anonymous""></script>
            <script src=""https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/js/bootstrap.min.js"" integrity=""sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl"" 
                  crossorigin=""anonymous""> </script>
        </body>
    </html> 
        ";

    return Results.Text(html, "text/html");
});

app.Run();

public class ImageFile
{
    public string? id { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
}

