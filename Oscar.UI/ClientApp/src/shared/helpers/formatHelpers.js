export function formatProductionTitle(production) {
    return production.title + ((production.year || production.productionType) ? ' (' + (production.year ? production.year : '') + (production.year && production.productionType ? ' ' : '') + (production.productionType === 'OneOff' ? 'One-Off' : (production.productionType === 'Series' ? 'Series' : '')) + ')' : '');
}

export function formatPersonName(person) {
    return person.forename + (person.forename ? " " : "") + (person.middleNames || '') + (person.middleNames ? " " : "") + person.surname;
}