using System;
using System.Collections.Generic;
using System.Text;

namespace GetTypetalkState.OuputLayout
{
    public interface ILayoutRepository
    {
        ILayout Get(string name);
    }

    public class LayoutRepository: ILayoutRepository
    {
        private readonly Dictionary<string, ILayout> _layoutRepository;
        private readonly ILayout _defaultLayout = new JsonLayout();

        public LayoutRepository()
        {
            _layoutRepository = new Dictionary<string, ILayout>
            {
                {"json", new JsonLayout()},
                { "table", new TableLayout() }
            };
        }

        public ILayout Get(string name)
        {
            var _ = string.IsNullOrEmpty(name) ? "json" : name;

            if (_layoutRepository == null || !_layoutRepository.ContainsKey(_))
            {
                return _defaultLayout;
            }
            return _layoutRepository[_];
        }
    }
}
