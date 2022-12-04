#include <iostream>
#include <fstream>
#include <vector>
#include <functional>
#include <string>
#include <algorithm>

using elf_t = std::vector<int>;
using elves_t = std::vector<elf_t>;
using lines_t = std::vector<std::string>;
using read_lines_f = std::function<lines_t(void)>;
using group_lines_f = std::function<elves_t(lines_t const&)>;
using count_calories_f = std::function<int(elf_t const&)>;
using get_fattest_elf_f = std::function<int(elves_t const&)>;
using sort_elf_calories_f = std::function<std::vector<int>(elves_t const&)>;

read_lines_f test_read_lines = []()
{
    return lines_t{
        "1000",
        "2000",
        "3000",
        "",
        "4000",
        "",
        "5000",
        "6000",
        "",
        "7000",
        "8000",
        "9000",
        "",
        "10000",
    };
};

lines_t real_read_lines()
{
    lines_t ret{};

    std::ifstream input{"input.txt"};
    std::string line{};

    while (std::getline(input, line))
    {
        ret.push_back(line);
    }

    return std::move(ret);
}

bool is_line_empty(std::string const& line)
{
    return line == "";
}

bool is_elf_empty(elf_t const& elf)
{
    return elf.size() == 0;
}

elves_t group_lines(lines_t const& lines)
{
    elves_t ret{};
    ret.emplace_back();
    int elf = 0;

    for (auto const& line : lines)
    {

        if (is_line_empty(line))
        {
            if (!is_elf_empty(ret[elf]))
            {
                ret.emplace_back();
                elf ++;
            }
        }
        else
        {
            int item = std::stoi(line);
            ret[elf].push_back(item);
        }
    }

    return std::move(ret);
}

int count_calories(elf_t const& elf)
{
    int sum{};

    for (auto const& item : elf)
    {
        sum += item;
    }

    return sum;
}

int find_fattest_elf(elves_t const& elves)
{
    int max{};
    int idx{};
    int max_idx{};

    for (auto const& elf : elves)
    {
        int calories = count_calories(elf);

        if (calories > max)
        {
            max = calories;
            max_idx = idx;
        }
        idx++;
    }

    return max_idx;
}

std::vector<int> sort_elf_calories(elves_t const& elves)
{
    std::vector<int> ret{};

    for (auto const& elf : elves)
    {
        ret.push_back(count_calories(elf));
    }

    std::sort(ret.begin(), ret.end());
    return std::move(ret);
}

int main()
{
    lines_t lines = real_read_lines();
    elves_t elves = group_lines(lines);
    auto sorted = sort_elf_calories(elves);

    auto most = sorted.end() - 1;

    std::cout << *most << std::endl;
    std::cout << *most + *(most - 1) + *(most - 2) << std::endl;
}
