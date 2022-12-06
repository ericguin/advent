use std::fs::File;
use std::io::{self, BufRead};
use std::path::Path;

fn read_lines<P>(filename: P) -> io::Result<io::Lines<io::BufReader<File>>>
where P: AsRef<Path>, {
    let file = File::open(filename)?;
    Ok(io::BufReader::new(file).lines())
}

struct Rucksack {
    contents: String
}

impl Rucksack {
    fn new(
}

fn main() -> io::Result<()> {
    let input = read_lines("input.txt")?;

    for line in input {
        if let Ok(contents) = line {
            let sack: Rucksack = Rucksack { contents };
        }
    }

    return Ok(());
}
