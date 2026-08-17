import moment from 'moment';

let xlsx;

export async function buildExcel(data, fileName, sheetName) {
    if (!xlsx) {
        xlsx = await import('xlsx');
    }

    var ws = xlsx.utils.json_to_sheet(data);

    /* add to workbook */
    var wb = xlsx.utils.book_new();
    xlsx.utils.book_append_sheet(wb, ws, sheetName.substring(0, 32));

    /* generate an XLSX file */
    xlsx.writeFile(wb, fileName + ".xlsx");
}

export function productionTransmissionSheet(data) {
        return data.map(x => x.canSeeAll ? productionTransmissionMapper(x) : productionLimitedTransmissionMapper(x));
}

function productionLimitedTransmissionMapper(x) {
    return {
        "Production Title": x.mainTitle,
        "Record Title": x.episodeTitle,
        "Transmission Production Title": x.transmissionTitle,
        "Transmission Episode Title": x.transmissionEpisodeTitle,
        "Broadcast Date": moment(x.broadcastDateTime).format('DD/MM/YYYY'),
        "Broadcast Time": moment(x.broadcastDateTime).format('hh:mm'),
        "Broadcast Duration": x.duration,
        "Broadcast language": x.broadcastLanguage,
        "Channel": x.channel,
        "Territories": x.territories.join('|'),
        "Genres": x.genres.join('|')
    };
}

function productionTransmissionMapper(x) {
    return {
        "Production Title": x.mainTitle,
        "Alt Production Titles": x.altProductionTitles.join('|'),
        "Record Title": x.episodeTitle,
        "Alt Record Titles": x.altEpisodeTitles.join('|'),
        "Transmission Production Title": x.transmissionTitle,
        "Transmission Episode Title": x.transmissionEpisodeTitle,
        "Broadcast Date": moment(x.broadcastDateTime).format('DD/MM/YYYY'),
        "Broadcast Time": moment(x.broadcastDateTime).format('HH:mm'),
        "Broadcast Duration": x.duration,
        "Broadcast language": x.broadcastLanguage,
        "Channel": x.channel,
        "Territories": x.territories.join('|'),
        "Production Year": x.productionYear,
        "Season Number": x.seasonNumber,
        "Episode Number": x.episodeNumber,
        "Genres": x.genres.join('|'),
        "Production Languages": x.productionLanguages.join('|'),
        "Production Countries": x.productionCountries.join('|'),
        "Production Company 1": GetIndex(0, x.productionCompanies),
        "Production Company 2": GetIndex(1, x.productionCompanies),
        "Production Company 3": GetIndex(2, x.productionCompanies),
        "Production Company 4": GetIndex(3, x.productionCompanies),
        "Production Company 5": GetIndex(4, x.productionCompanies),
        "Actor 1": GetIndex(0, x.actors),
        "Actor 2": GetIndex(1, x.actors),
        "Actor 3": GetIndex(2, x.actors),
        "Actor 4": GetIndex(3, x.actors),
        "Actor 5": GetIndex(4, x.actors),
        "Actor 6": GetIndex(5, x.actors),
        "Actor 7": GetIndex(6, x.actors),
        "Actor 8": GetIndex(7, x.actors),
        "Director 1": GetIndex(0, x.directors),
        "Director 2": GetIndex(1, x.directors),
        "Director 3": GetIndex(2, x.directors),
        "Director 4": GetIndex(3, x.directors),
        "Director 5": GetIndex(4, x.directors),
        "Producer 1": GetIndex(0, x.producers),
        "Producer 2": GetIndex(1, x.producers),
        "Producer 3": GetIndex(2, x.producers),
        "Producer 4": GetIndex(3, x.producers),
        "Writer 1": GetIndex(0, x.writers),
        "Writer 2": GetIndex(1, x.writers),
        "Writer 3": GetIndex(2, x.writers),
        "Creator 1": GetIndex(0, x.creators),
        "Creator 2": GetIndex(1, x.creators),
    };
}

function GetIndex(index, array) {
    if (array && array.length > index && index >= 0) {
        return array[index];
    }

    return '';
}