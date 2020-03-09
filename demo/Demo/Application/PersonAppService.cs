using SimpleDynamicWebAPI;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Application
{
    public class CreateUpdatePersonInput
    {
        public string Name { get; set; }
    }

    public class PersonDto
    {
        public string Name { get; set; }
    }

    public class PersonAppService : IApplicationService
    {
        public string Create(CreateUpdatePersonInput input)
        {
            return $"你造了个名字叫：{input.Name} 的人";
        }

        public string Delete(int id)
        {
            return $"你把 Id：{id} 的人干掉了";
        }

        public string Get(int id)
        {
            return $"你输入的 Id 是：{id}";
        }

        public List<PersonDto> GetAll()
        {
            return "服务器向你扔了一堆人"
                .ToCharArray()
                .Select(temp => new PersonDto
                {
                    Name = temp.ToString()
                })
                .ToList();
        }

        public string Update(int id, CreateUpdatePersonInput input)
        {
            return $"你把 Id：{id} 的人的名字改成了 {input.Name}";
        }
    }
}