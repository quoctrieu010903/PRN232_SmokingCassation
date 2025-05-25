using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Domain.Interfaces
{
    public interface IGenericRepository<TEntity > where TEntity : class
    {
        /// <summary>
        /// Tìm và trả về một thực thể dựa trên khóa chính (ID).
        /// </summary>
        /// <param name="id">ID của thực thể cần tìm.</param>
        /// <param name="cancellationToken">Token để hủy bỏ tác vụ bất đồng bộ nếu cần.</param>
        /// <param name="includeProperties">Danh sách các thuộc tính dẫn hướng cần include (load kèm theo).</param>
        /// <returns>Thực thể tương ứng với ID nếu tìm thấy; ngược lại trả về null.</returns>
        Task<TEntity> FindByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Tìm một thực thể duy nhất thỏa mãn điều kiện cho trước.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện để lọc thực thể (có thể null để lấy thực thể bất kỳ đầu tiên).</param>
        /// <param name="cancellationToken">Token để hủy bỏ tác vụ bất đồng bộ nếu cần.</param>
        /// <param name="includeProperties">Danh sách các thuộc tính dẫn hướng cần include (load kèm theo).</param>
        /// <returns>Thực thể thỏa mãn điều kiện nếu có; ngược lại trả về null.</returns>
        Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Tìm tất cả các thực thể thỏa mãn điều kiện cho trước.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện để lọc thực thể (có thể null để lấy tất cả).</param>
        /// <param name="includeProperties">Danh sách các thuộc tính dẫn hướng cần include (load kèm theo).</param>
        /// <returns>Tập truy vấn (IQueryable) các thực thể thỏa mãn điều kiện.</returns>
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);


        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        void RemoveMultiple(List<TEntity> entities);
    }
}
