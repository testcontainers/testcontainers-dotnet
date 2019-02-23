namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using static LanguageExt.Prelude;

  internal sealed class MetaDataClientImages : DockerMetaDataClient<ImagesListResponse>
  {
    private static readonly Lazy<DockerMetaDataClient<ImagesListResponse>> MetaDataClient = new Lazy<DockerMetaDataClient<ImagesListResponse>>(() =>
    {
      return new MetaDataClientImages();
    });

    private MetaDataClientImages()
    {
    }

    internal static DockerMetaDataClient<ImagesListResponse> Instance
    {
      get
      {
        return MetaDataClient.Value;
      }
    }

    internal override IReadOnlyCollection<ImagesListResponse> GetAll()
    {
      return Docker.Images.ListImagesAsync(new ImagesListParameters { All = true }).Result.ToList();
    }

    internal override ImagesListResponse ById(string id)
    {
      return notnull(id) ? this.GetAll().FirstOrDefault(value => id.Equals(value.ID)) : null;
    }

    internal override ImagesListResponse ByName(string name)
    {
      return this.ByProperty("label", name);
    }

    internal override ImagesListResponse ByProperty(string property, string value)
    {
      return Docker.Images.ListImagesAsync(new ImagesListParameters
      {
        All = true,
        Filters = new Dictionary<string, IDictionary<string, bool>>
        {
          {
            property, new Dictionary<string, bool>
            {
              { value, true },
            }
          },
        },
      }).Result.FirstOrDefault();
    }
  }
}
