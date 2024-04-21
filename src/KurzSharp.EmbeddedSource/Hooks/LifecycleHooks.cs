namespace KurzSharp;

public class LifecycleHooks<TDto>
{
    public virtual TDto OnBeforeCreate(TDto dto) => dto;
    public virtual IEnumerable<TDto> OnBeforeAllRead(IEnumerable<TDto> dtos) => dtos;
    public virtual TDto OnBeforeRead(TDto dto) => dto;
    public virtual TDto OnBeforeUpdate(TDto dto) => dto;
    public virtual TDto OnBeforeDelete(TDto dto) => dto;
}
