#![allow(unused_imports)]
use std::fs::File;
use std::io::{self, prelude::*, BufReader};

fn open_file_buffer(name: &str) -> io::Result<BufReader<File>> {
    let f = File::open(name)?;
    Ok(BufReader::new(f))
}

fn parse_elves_from_file(buffer: BufReader<File>) -> io::Result<Vec<u32>> {
    let mut elves : Vec<u32> = Vec::new();
    let mut calories : u32 = 0;

    for line in buffer.lines().filter_map(|l| l.ok()) {
        if line.is_empty() {
            elves.push(calories);
            calories = 0;
        }
        else {
            calories += line.parse::<u32>().expect("Line wasn't a valid int");
        }
    }

    Ok(elves)
}

fn main() {

    open_file_buffer("input.txt").and_then(parse_elves_from_file)
        .map(|e| e.iter().max())
        .map(|m| println!("Max is {}", m.expect("Failed to parse elves")));
}
