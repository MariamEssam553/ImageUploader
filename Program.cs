
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return Results.Text($@"<!DOCTYPE html>
    <html lang=""en"">
    <head>
        <title> Image Uploader </title>
        <meta charset=""utf-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css"" integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" crossorigin=""anonymous"">
    </head>
    <body>
    <div class=""container d-flex justify-content-center"" style=""height: 100vh;align-items: center;"">

    <div class=""card"" style=""width: 60rem;height: 26rem; background-color: aliceblue; box-shadow:0px 0px 10px 0px #455a6466;"">
        <div class=""card-header text-center font-weight-bold"" style=""background-color:#b7d4ed; font-size: x-large;color: #E91E63;""> Image Uploader </div>
    
    <form action=""/upload"" method=""POST"" style=""height: 22rem; display: flex; width: 80%;align-self: center; "">
      <div class=""card-body"" style=""display: flex; flex-direction: column;justify-content: space-evenly"">
      <div class=""form-group"">
        <input type=""text"" class=""form-control is-invalid"" id=""title"" name=""title"" placeholder=""Enter image title""> 
        <div id=""validationServer03Feedback"" class=""invalid-feedback""> Please enter a title for the image</div>
      </div>

      <div class=""input-group mb-3"">
        <div class=""custom-file"">
          <input type=""file"" class=""custom-file-input"" id=""image"">
          <label class=""custom-file-label"" for=""image"">Choose image...</label>
        </div>
      </div>

      <button class=""btn btn-outline-dark"" style=""font-weight: 700;border-width: medium; width: fit-content;align-self: flex-end;"" type=""submit"">Submit</button>
      
    
      </div> 
    </form>
    
    
       <script src=""https://code.jquery.com/jquery-3.2.1.slim.min.js"" integrity=""sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"" crossorigin=""anonymous""></script>
       <script src=""https://cdn.jsdelivr.net/npm/popper.js@1.12.9/dist/umd/popper.min.js"" integrity=""sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"" crossorigin=""anonymous""></script>
       <script src=""https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/js/bootstrap.min.js"" integrity=""sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl"" crossorigin=""anonymous""></script>
    
    </body>
    </html>
    ", "text/html");
});

app.Run();

