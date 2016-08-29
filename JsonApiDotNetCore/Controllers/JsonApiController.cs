using JsonApiDotNetCore.Abstractions;
using JsonApiDotNetCore.Data;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Collections.Generic;

namespace JsonApiDotNetCore.Controllers
{
  public class JsonApiController : IJsonApiController
  {
    protected readonly IJsonApiContext JsonApiContext;
    private readonly ResourceRepository _resourceRepository;

    public JsonApiController(IJsonApiContext jsonApiContext, ResourceRepository resourceRepository)
    {
      JsonApiContext = jsonApiContext;
      _resourceRepository = resourceRepository;
    }

    public virtual ObjectResult Get()
    {
      var entities = _resourceRepository.Get();
      if(entities == null) {
        return new NotFoundObjectResult(null);
      }
      return new OkObjectResult(entities);
    }

    public virtual ObjectResult Get(string id)
    {
      var entity = _resourceRepository.Get(id);
      if(entity == null) {
        return new NotFoundObjectResult(null);
      }
      return new OkObjectResult(entity);
    }

    public virtual ObjectResult Post(object entity)
    {
      _resourceRepository.Add(entity);
      _resourceRepository.SaveChanges();
      return new CreatedResult(JsonApiContext.HttpContext.Request.Path, entity);
    }

    public virtual ObjectResult Patch(string id, Dictionary<PropertyInfo, object> entityPatch)
    {
      var entity = _resourceRepository.Get(id);
      if(entity == null) {
        return new NotFoundObjectResult(null);
      }

      entity = PatchEntity(entity, entityPatch);
      _resourceRepository.SaveChanges();

      return new OkObjectResult(entity);
    }

    public virtual ObjectResult Delete(string id)
    {
      _resourceRepository.Delete(id);
      _resourceRepository.SaveChanges();
      return new OkObjectResult(null);
    }

    protected object PatchEntity(object entity, Dictionary<PropertyInfo, object> entityPatch)
    {
      foreach(var attrPatch in entityPatch)
      {
        attrPatch.Key.SetValue(entity, attrPatch.Value);
      }

      return entity;
    }
  }
}
